using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Poker;

public sealed class PokerEvaluator
{
    internal const ulong AceHighRankMask = 1UL << 13;
    private const ulong KingRankMask = 1UL << 12;
    private const ulong TenRankMask = 1UL << 9;
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

        return hand.Combinations(5).Max(EvaluateFiveCardHandInternal)!;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PokerHand EvaluateFiveCardHand(IReadOnlyCollection<Card> hand)
    {
        if (hand.Count != 5)
        {
            throw new ArgumentException("Value must have 5 cards.", nameof(hand));
        }

        return EvaluateFiveCardHandInternal(hand);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand EvaluateFiveCardHandInternal(IReadOnlyCollection<Card> hand)
    {
        var handMask = hand.Aggregate(0UL, (current, card) => current | card.BitMask);

        // If we reduce with OR then the count is the number of different ranks we have.
        // Flush/straight/high card -> 5x1 -> count = 5
        // Four of a kind -> 1x4, 1x1 -> count = 2
        // Full house -> 1x3, 1x2 -> count = 2
        // Three of a kind -> 1x3, 2x1 -> count = 3
        // Two pair -> 2x2, 1x1 -> count = 3
        // Pair -> 1x2, 3x1 -> count = 4
        var orReduction = handMask.HorizontalOr16();
        var orReductionCount = orReduction.PopCount();

        // Test the most common branches first.
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

        return new PokerHand(PokerHandType.FourOfAKind, MoveLowAceToHigh(andReduced), MoveLowAceToHigh(highCardMask));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FullHouse(ulong handMask, ulong orReduction)
    {
        // OR reduced contains two bits, one for the three and one for the two. XOR reduction will give us
        // a bit for the three because it is an odd length sequence.
        var threeMask = handMask.HorizontalXor16();

        var pairMask = orReduction ^ threeMask;

        return new PokerHand(PokerHandType.FullHouse, MoveLowAceToHigh(threeMask), MoveLowAceToHigh(pairMask));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand Pair(ulong handMask, ulong orReduction)
    {
        // XOR reduction will give us a mask for the three single card ranks because they are the odd lengthened sequences.
        var hardCardsRankMask = handMask.HorizontalXor16();

        var pairRankMask = orReduction ^ hardCardsRankMask;

        return new PokerHand(PokerHandType.Pair, MoveLowAceToHigh(pairRankMask), MoveLowAceToHigh(hardCardsRankMask));
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

        // Try SHD and HDC first as they require less operations.
        var threeMask = (handMask >> 16) & sdAndHcRankMasks;

        if (threeMask == 0)
        {
            // Must be SDC or SHC.
            var cAndSRankMask = Bits0To31 & ((handMask << 16) | (handMask >> 48));

            threeMask = cAndSRankMask & sdAndHcRankMasks;
        }

        var threeRankMask = Bits0To15 & (threeMask | (threeMask >> 16));

        var highCardsRankMask = orReduction ^ threeRankMask;

        return new PokerHand(PokerHandType.ThreeOfAKind, MoveLowAceToHigh(threeRankMask), MoveLowAceToHigh(highCardsRankMask));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand TwoPair(ulong orReduction, ulong xorReduction)
    {
        var pairMask = orReduction ^ xorReduction;

        return new PokerHand(PokerHandType.TwoPair, MoveLowAceToHigh(pairMask), MoveLowAceToHigh(xorReduction));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FlushStraightOrHighCard(ulong handMask, ulong orReduction)
    {
        var rankMask = orReduction;
        var hasAce = (orReduction & 1UL) != 0;

        if (hasAce)
        {
            // Add an extra bit to represent the ace as high - allows us to do the same run test
            // for AK as we would for 2A.
            rankMask |= AceHighRankMask;
        }

        // If we AND the rank mask with itself shifted right one then we will get a 1 bit everywhere
        // there is a run of two cards, with the bit being on the lower of the two cards.
        var runOfTwoRankMask = rankMask & (rankMask >> 1);

        // For a run of five we must have four runs of two. These can only be consecutive *except* when
        // we have an ace because we've added the extra high bit for an ace. We can distinguish this 
        // case easily because there will be a bit in both the king and low ace positions.
        var twoRunRankMaskCount = runOfTwoRankMask.PopCount();
        if (twoRunRankMaskCount == 4 &&
            (!hasAce ||
             (runOfTwoRankMask & 1UL) == 0 ||
             (runOfTwoRankMask & KingRankMask) == 0))
        {
            return Straight(handMask, runOfTwoRankMask);
        }

        return FlushOrHighCard(handMask, orReduction);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand FlushOrHighCard(ulong handMask, ulong orReduction)
    {
        var type = IsSingleSuit(handMask) ? PokerHandType.Flush : PokerHandType.HighCard;

        return new PokerHand(type, MoveLowAceToHigh(orReduction));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static PokerHand Straight(ulong handMask, ulong twoRunRankMask)
    {
        // twoRunRankMask & -twoRunRankMask ?
        var rankMask = twoRunRankMask ^ (twoRunRankMask & (twoRunRankMask - 1));

        var isSingleSuit = IsSingleSuit(handMask);

        // We can shift the low rank left 4 to get the high rank *except* for a 10 as that would
        // put it in the ace high position.
        if (rankMask == TenRankMask)
        {
            rankMask = AceHighRankMask;
        }
        else
        {
            rankMask <<= 4;
        }

        if (isSingleSuit)
        {
            if (rankMask == AceHighRankMask)
            {
                return new PokerHand(PokerHandType.RoyalFlush, AceHighRankMask);
            }

            return new PokerHand(PokerHandType.StraightFlush, rankMask);
        }

        return new PokerHand(PokerHandType.Straight, rankMask);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static bool IsSingleSuit(ulong handMask)
    {
        return (handMask & Bits0To15) == handMask ||
               (handMask & Bits16To31) == handMask ||
               (handMask & Bits32To47) == handMask ||
               (handMask & Bits48To63) == handMask;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong MoveLowAceToHigh(ulong handMask)
    {
        var ace = handMask & 1;
        var aceHigh = ace << 13;
        var withoutAce = handMask & 0b1111111111111110;

        return withoutAce | aceHigh;
    }
}