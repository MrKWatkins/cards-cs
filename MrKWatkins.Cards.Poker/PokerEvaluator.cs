using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Poker;

[JetBrains.Annotations.PublicAPI]
public sealed class PokerEvaluator
{
    private const ulong AceHighRankMask = 1UL << 13;
    private const ulong Bits0To15 = 0x000000000000FFFF;
    private const ulong Bits0To31 = 0x00000000FFFFFFFF;
    private const ulong Bits16To31 = 0x00000000FFFF0000;
    private const ulong Bits32To47 = 0x0000FFFF00000000;
    private const ulong Bits48To63 = 0xFFFF000000000000;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PokerHand EvaluateSevenCardHand(IReadOnlyCollection<Card> hand)
    {
        if (hand.Count != 7)
        {
            throw new ArgumentException("Value must have 7 cards.", nameof(hand));
        }

        PokerHand? result = null;
        foreach (var fiveCardHand in hand.Combinations(5))
        {
            var handMask = 0UL;
            // ReSharper disable once LoopCanBeConvertedToQuery - faster to not use LINQ.
            foreach (var card in fiveCardHand)
            {
                handMask |= card.AceHighBitMask;
            }

            var fiveCardResult = EvaluateFiveCardHand(handMask);
            if (result == null || result < fiveCardResult)
            {
                result = fiveCardResult;
            }
        }
        return result!.Value;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PokerHand EvaluateFiveCardHand([JetBrains.Annotations.InstantHandle] IEnumerable<Card> hand)
    {
        var handMask = 0UL;
        var count = 0;
        foreach (var card in hand)
        {
            handMask |= card.AceHighBitMask;
            count++;
        }

        if (count != 5)
        {
            throw new ArgumentException("Value must have 5 cards.", nameof(hand));
        }

        return EvaluateFiveCardHand(handMask);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand EvaluateFiveCardHand(ulong handMask)
    {
        // If we reduce with OR then the count is the number of different ranks we have.
        // Flush/straight/high card -> 5x1 -> count = 5
        // Four of a kind -> 1x4, 1x1 -> count = 2
        // Full house -> 1x3, 1x2 -> count = 2
        // Three of a kind -> 1x3, 2x1 -> count = 3
        // Two pair -> 2x2, 1x1 -> count = 3
        // Pair -> 1x2, 3x1 -> count = 4
        var orReduction = handMask.HorizontalOr16();
        var orReductionCount = orReduction.PopCount();

        // Test the most common branches first; therefore not using a switch as we cannot guarantee the order of evaluation.
        // Benchmarks with a switch expression showed it was slightly slower. Also tried an array of functions with a straight
        // lookup into that by orReductionCount, that was slower still.
        if (orReductionCount == 5)
        {
            return FlushStraightOrHighCard(handMask, orReduction);
        }

        if (orReductionCount == 4)
        {
            return Pair(handMask, orReduction);
        }

        if (orReductionCount == 3)
        {
            return ThreeOfAKindOrTwoPair(handMask, orReduction);
        }

        return FourOfAKindOrFullHouse(handMask, orReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FourOfAKindOrFullHouse(ulong handMask, ulong orReduction)
    {
        var andReduced = handMask.HorizontalAnd16();

        if (andReduced != 0UL)
        {
            return FourOfAKind(orReduction, andReduced);
        }

        return FullHouse(handMask, orReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FourOfAKind(ulong orReduction, ulong andReduced)
    {
        // AND reduced contains a single bit which is the four. OR reduced contains two bits,
        // one for the four and one for the high card. Four is in both, high card in one - XOR
        // will extract it.
        var highCardMask = orReduction ^ andReduced;

        return new PokerHand(PokerHandType.FourOfAKind, andReduced, highCardMask);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FullHouse(ulong handMask, ulong orReduction)
    {
        // OR reduced contains two bits, one for the three and one for the two. XOR reduction will give us
        // a bit for the three because it is an odd length sequence.
        var threeMask = handMask.HorizontalXor16();

        var pairMask = orReduction ^ threeMask;

        return new PokerHand(PokerHandType.FullHouse, threeMask, pairMask);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand Pair(ulong handMask, ulong orReduction)
    {
        // XOR reduction will give us a mask for the three single card ranks because they are the odd lengthened sequences.
        var hardCardsRankMask = handMask.HorizontalXor16();

        var pairRankMask = orReduction ^ hardCardsRankMask;

        return new PokerHand(PokerHandType.Pair, pairRankMask, hardCardsRankMask);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand ThreeOfAKindOrTwoPair(ulong handMask, ulong orReduction)
    {
        // XOR reduction gives a 1 for each odd length sequence.
        // Three of a kind -> 1x3, 2x1 -> count = 3
        // Two pair -> 2x2, 1x1 -> count = 1; 1 bit is high card
        var xorReduction = handMask.HorizontalXor16();
        var xorReductionCount = xorReduction.PopCount();

        if (xorReductionCount == 3)
        {
            return ThreeOfAKind(handMask, orReduction);
        }

        return TwoPair(orReduction, xorReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand ThreeOfAKind(ulong handMask, ulong orReduction)
    {
        // There are four combinations of three suits: SHD, SHC, SDC, HDC

        // All either have SD or HC.
        var sdAndHcRankMasks = handMask & (handMask >> 32);

        // We now have a bit corresponding to the three card rank in bits 0 -> 15 or bits 16 -> 31. Isolate it.
        var threeRankMask = ((sdAndHcRankMasks >> 16) | sdAndHcRankMasks) & Bits0To15;

        var highCardsRankMask = orReduction ^ threeRankMask;

        return new PokerHand(PokerHandType.ThreeOfAKind, threeRankMask, highCardsRankMask);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand TwoPair(ulong orReduction, ulong xorReduction)
    {
        var pairMask = orReduction ^ xorReduction;

        return new PokerHand(PokerHandType.TwoPair, pairMask, xorReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FlushStraightOrHighCard(ulong handMask, ulong orReduction)
    {
        // As we are using aces high we need a special case for 5 -> A straight.
        if (orReduction == 0b10000000011110)
        {
            return new PokerHand(IsSingleSuit(handMask) ? PokerHandType.StraightFlush : PokerHandType.Straight, 0b00000000011111);
        }
        
        // If we AND the rank mask with itself shifted right one then we will get a 1 bit everywhere
        // there is a run of two cards, with the bit being on the lower of the two cards.
        var runOfTwoRankMask = orReduction & (orReduction >> 1);

        // For a run of five we must have four runs of two. These can only be consecutive.
        if (runOfTwoRankMask.PopCount() == 4)
        {
            return Straight(handMask, orReduction);
        }

        return FlushOrHighCard(handMask, orReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FlushOrHighCard(ulong handMask, ulong orReduction)
    {
        var type = IsSingleSuit(handMask) ? PokerHandType.Flush : PokerHandType.HighCard;

        return new PokerHand(type, orReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand Straight(ulong handMask, ulong orReduction)
    {        
        if (IsSingleSuit(handMask))
        {
            var type = (orReduction & AceHighRankMask) == AceHighRankMask ? PokerHandType.RoyalFlush : PokerHandType.StraightFlush;
            
            return new PokerHand(type, orReduction);
        }

        return new PokerHand(PokerHandType.Straight, orReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static bool IsSingleSuit(ulong handMask) =>
        // Performs better using | rather than || as it avoids branching, branch prediction, etc.
        (handMask & Bits0To15) == handMask |
        (handMask & Bits16To31) == handMask |
        (handMask & Bits32To47) == handMask |
        (handMask & Bits48To63) == handMask;
}