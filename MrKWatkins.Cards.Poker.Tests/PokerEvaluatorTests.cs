using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using FluentAssertions;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Poker.Tests;

[TestFixture]
public sealed class PokerEvaluatorTests
{
    [TestCase("AS KS QS JS 10S 9S")]
    [TestCase("AS KS QS JS")]
    public void EvaluateFiveCardHand_ThrowsIfHandDoesNotHaveFiveCards(string hand)
    {
        var cardSet = new CardSet(CardFormat.Default.ParseMultipleOrThrow(hand));
        var evaluator = new PokerEvaluator();
        evaluator.Invoking(p => p.EvaluateFiveCardHand(cardSet)).Should().Throw<ArgumentException>();
    }

    // Hand type comes first so hands are organised by type in test runners.
    [TestCase(PokerHandType.RoyalFlush, "AS KS QS JS 10S", new[] { Rank.Ace, Rank.King, Rank.Queen, Rank.Jack, Rank.Ten }, null)]

    [TestCase(PokerHandType.FourOfAKind, "AS AH AD AC KH", new[] { Rank.Ace }, new[] { Rank.King })]
    [TestCase(PokerHandType.FourOfAKind, "4S 4H 4D 4C 7H", new[] { Rank.Four }, new[] { Rank.Seven })]

    [TestCase(PokerHandType.StraightFlush, "KS QS JS 10S 9S", new[] { Rank.King, Rank.Queen, Rank.Jack, Rank.Ten, Rank.Nine }, null)]
    [TestCase(PokerHandType.StraightFlush, "QH JH 10H 9H 8H", new[] { Rank.Queen, Rank.Jack, Rank.Ten, Rank.Nine, Rank.Eight }, null)]
    [TestCase(PokerHandType.StraightFlush, "9D 8D 7D 6D 5D", new[] { Rank.Nine, Rank.Eight, Rank.Seven, Rank.Six, Rank.Five }, null)]
    [TestCase(PokerHandType.StraightFlush, "5C 4C 3C 2C AC", new[] { Rank.Five, Rank.Four, Rank.Three, Rank.Two, Rank.Ace }, null)]

    [TestCase(PokerHandType.FullHouse, "4S 4H 4D 8S 8H", new[] { Rank.Four }, new[] { Rank.Eight })] // 3 = SHD
    [TestCase(PokerHandType.FullHouse, "KS KH KC AD AH", new[] { Rank.King }, new[] { Rank.Ace })] // 3 = SHC
    [TestCase(PokerHandType.FullHouse, "3S 3D 3C 2S 2C", new[] { Rank.Three }, new[] { Rank.Two })] // 3 = SDC
    [TestCase(PokerHandType.FullHouse, "JH JD JC 9H 9D", new[] { Rank.Jack }, new[] { Rank.Nine })] // 3 = HDC

    [TestCase(PokerHandType.Flush, "KS QS JS 9S 8S", new[] { Rank.King, Rank.Queen, Rank.Jack, Rank.Nine, Rank.Eight }, null)]
    [TestCase(PokerHandType.Flush, "AH JH 7H 4H 2H", new[] { Rank.Ace, Rank.Jack, Rank.Seven, Rank.Four, Rank.Two }, null)]
    [TestCase(PokerHandType.Flush, "9D 8D 7D 5D 3D", new[] { Rank.Nine, Rank.Eight, Rank.Seven, Rank.Five, Rank.Three }, null)]
    [TestCase(PokerHandType.Flush, "8C 7C 4C 3C 2C", new[] { Rank.Eight, Rank.Seven, Rank.Four, Rank.Three, Rank.Two }, null)]

    [TestCase(PokerHandType.Straight, "KS QS JS 10H 9S", new[] { Rank.King, Rank.Queen, Rank.Jack, Rank.Ten, Rank.Nine }, null)]
    [TestCase(PokerHandType.Straight, "QH JH 10H 9D 8H", new[] { Rank.Queen, Rank.Jack, Rank.Ten, Rank.Nine, Rank.Eight }, null)]
    [TestCase(PokerHandType.Straight, "9D 8D 7D 6C 5D", new[] { Rank.Nine, Rank.Eight, Rank.Seven, Rank.Six, Rank.Five }, null)]
    [TestCase(PokerHandType.Straight, "5C 4C 3C 2S AC", new[] { Rank.Five, Rank.Four, Rank.Three, Rank.Two, Rank.Ace }, null)]

    [TestCase(PokerHandType.ThreeOfAKind, "4S 4H 4D 8S 6H", new[] { Rank.Four }, new[] { Rank.Eight, Rank.Six })] // SHD
    [TestCase(PokerHandType.ThreeOfAKind, "10S 10H 10C AC 9D", new[] { Rank.Ten }, new[] { Rank.Ace, Rank.Nine })] // SHC
    [TestCase(PokerHandType.ThreeOfAKind, "9S 9D 9C 3H 2D", new[] { Rank.Nine }, new[] { Rank.Three, Rank.Two })] // SDC
    [TestCase(PokerHandType.ThreeOfAKind, "7H 7D 7C QH 5C", new[] { Rank.Seven }, new[] { Rank.Queen, Rank.Five })] // HDC

    [TestCase(PokerHandType.TwoPair, "QS QH 10S 10H AC", new[] { Rank.Queen, Rank.Ten }, new[] { Rank.Ace })] // SH & HD 
    [TestCase(PokerHandType.TwoPair, "QS QH 10S 10D AC", new[] { Rank.Queen, Rank.Ten }, new[] { Rank.Ace })] // SH & HD 
    [TestCase(PokerHandType.TwoPair, "QS QH 10S 10D AC", new[] { Rank.Queen, Rank.Ten }, new[] { Rank.Ace })] // SH & HD 
    [TestCase(PokerHandType.TwoPair, "QS QH 10H 10D AC", new[] { Rank.Queen, Rank.Ten }, new[] { Rank.Ace })] // SH & HD 
    [TestCase(PokerHandType.TwoPair, "QS QH 10H 10C AC", new[] { Rank.Queen, Rank.Ten }, new[] { Rank.Ace })] // SH & HC 
    [TestCase(PokerHandType.TwoPair, "QS QH 10D 10C AC", new[] { Rank.Queen, Rank.Ten }, new[] { Rank.Ace })] // SH & DC 
    [TestCase(PokerHandType.TwoPair, "JS JD 9S 9H 3C", new[] { Rank.Jack, Rank.Nine }, new[] { Rank.Three })] // SD & SH
    [TestCase(PokerHandType.TwoPair, "JS JD 9S 9D 3C", new[] { Rank.Jack, Rank.Nine }, new[] { Rank.Three })] // SD & SD
    [TestCase(PokerHandType.TwoPair, "JS JD 9S 9C 3C", new[] { Rank.Jack, Rank.Nine }, new[] { Rank.Three })] // SD & SC
    [TestCase(PokerHandType.TwoPair, "JS JD 9H 9D 3C", new[] { Rank.Jack, Rank.Nine }, new[] { Rank.Three })] // SD & HD
    [TestCase(PokerHandType.TwoPair, "JS JD 9H 9C 3C", new[] { Rank.Jack, Rank.Nine }, new[] { Rank.Three })] // SD & HC
    [TestCase(PokerHandType.TwoPair, "JS JD 9D 9C 3C", new[] { Rank.Jack, Rank.Nine }, new[] { Rank.Three })] // SD & DC
    [TestCase(PokerHandType.TwoPair, "QS QC JS JH 4H", new[] { Rank.Queen, Rank.Jack }, new[] { Rank.Four })] // SC & SH
    [TestCase(PokerHandType.TwoPair, "QS QC JS JD 4H", new[] { Rank.Queen, Rank.Jack }, new[] { Rank.Four })] // SC & SD
    [TestCase(PokerHandType.TwoPair, "QS QC JS JC 4H", new[] { Rank.Queen, Rank.Jack }, new[] { Rank.Four })] // SC & SC
    [TestCase(PokerHandType.TwoPair, "QS QC JH JD 4H", new[] { Rank.Queen, Rank.Jack }, new[] { Rank.Four })] // SC & HD
    [TestCase(PokerHandType.TwoPair, "QS QC JH JC 4H", new[] { Rank.Queen, Rank.Jack }, new[] { Rank.Four })] // SC & HC
    [TestCase(PokerHandType.TwoPair, "QS QC JD JC 4H", new[] { Rank.Queen, Rank.Jack }, new[] { Rank.Four })] // SC & DC
    [TestCase(PokerHandType.TwoPair, "9H 9D 7S 7H QH", new[] { Rank.Nine, Rank.Seven }, new[] { Rank.Queen })] // HD & SH
    [TestCase(PokerHandType.TwoPair, "9H 9D 7S 7D QH", new[] { Rank.Nine, Rank.Seven }, new[] { Rank.Queen })] // HD & SD
    [TestCase(PokerHandType.TwoPair, "9H 9D 7S 7C QH", new[] { Rank.Nine, Rank.Seven }, new[] { Rank.Queen })] // HD & SC
    [TestCase(PokerHandType.TwoPair, "9H 9D 7H 7D QH", new[] { Rank.Nine, Rank.Seven }, new[] { Rank.Queen })] // HD & HD
    [TestCase(PokerHandType.TwoPair, "9H 9D 7H 7C QH", new[] { Rank.Nine, Rank.Seven }, new[] { Rank.Queen })] // HD & HC
    [TestCase(PokerHandType.TwoPair, "9H 9D 7D 7C QH", new[] { Rank.Nine, Rank.Seven }, new[] { Rank.Queen })] // HD & DC
    [TestCase(PokerHandType.TwoPair, "JH JC 5S 5H AS", new[] { Rank.Jack, Rank.Five }, new[] { Rank.Ace })] // HC & SH
    [TestCase(PokerHandType.TwoPair, "JH JC 5S 5D AS", new[] { Rank.Jack, Rank.Five }, new[] { Rank.Ace })] // HC & SD
    [TestCase(PokerHandType.TwoPair, "JH JC 5S 5C AS", new[] { Rank.Jack, Rank.Five }, new[] { Rank.Ace })] // HC & SC
    [TestCase(PokerHandType.TwoPair, "JH JC 5H 5D AS", new[] { Rank.Jack, Rank.Five }, new[] { Rank.Ace })] // HC & HD
    [TestCase(PokerHandType.TwoPair, "JH JC 5H 5C AS", new[] { Rank.Jack, Rank.Five }, new[] { Rank.Ace })] // HC & HC
    [TestCase(PokerHandType.TwoPair, "JH JC 5D 5C AS", new[] { Rank.Jack, Rank.Five }, new[] { Rank.Ace })] // HC & DC
    [TestCase(PokerHandType.TwoPair, "8D 8C 2S 2H KH", new[] { Rank.Eight, Rank.Two }, new[] { Rank.King })] // DC & SH
    [TestCase(PokerHandType.TwoPair, "8D 8C 2S 2D KH", new[] { Rank.Eight, Rank.Two }, new[] { Rank.King })] // DC & SD
    [TestCase(PokerHandType.TwoPair, "8D 8C 2S 2C KH", new[] { Rank.Eight, Rank.Two }, new[] { Rank.King })] // DC & SC
    [TestCase(PokerHandType.TwoPair, "8D 8C 2H 2D KH", new[] { Rank.Eight, Rank.Two }, new[] { Rank.King })] // DC & HD
    [TestCase(PokerHandType.TwoPair, "8D 8C 2H 2C KH", new[] { Rank.Eight, Rank.Two }, new[] { Rank.King })] // DC & HC
    [TestCase(PokerHandType.TwoPair, "8D 8C 2D 2C KH", new[] { Rank.Eight, Rank.Two }, new[] { Rank.King })] // DC & DC

    [TestCase(PokerHandType.Pair, "6S 6H AH QH 9C", new[] { Rank.Six }, new[] { Rank.Ace, Rank.Queen, Rank.Nine })] // SH
    [TestCase(PokerHandType.Pair, "5S 5D 8H 6S 2C", new[] { Rank.Five }, new[] { Rank.Eight, Rank.Six, Rank.Two })] // SD
    [TestCase(PokerHandType.Pair, "3S 3C 8H 5C 2S", new[] { Rank.Three }, new[] { Rank.Eight, Rank.Five, Rank.Two })] // SC
    [TestCase(PokerHandType.Pair, "JH JD KS 10C 9C", new[] { Rank.Jack }, new[] { Rank.King, Rank.Ten, Rank.Nine })] // HD
    [TestCase(PokerHandType.Pair, "7H 7C AC KS JS", new[] { Rank.Seven }, new[] { Rank.Ace, Rank.King, Rank.Jack })] // HC
    [TestCase(PokerHandType.Pair, "2D 2C JS 10C 6C", new[] { Rank.Two }, new[] { Rank.Jack, Rank.Ten, Rank.Six })] // DC

    [TestCase(PokerHandType.HighCard, "AS KS QS JS 9C", new[] { Rank.Ace, Rank.King, Rank.Queen, Rank.Jack, Rank.Nine }, null)]
    [TestCase(PokerHandType.HighCard, "AS KS QS 10C 9C", new[] { Rank.Ace, Rank.King, Rank.Queen, Rank.Ten, Rank.Nine }, null)]
    [TestCase(PokerHandType.HighCard, "AS KS QS 3C 2C", new[] { Rank.Ace, Rank.King, Rank.Queen, Rank.Three, Rank.Two }, null)]
    [TestCase(PokerHandType.HighCard, "KS QS 6H 3C 2C", new[] { Rank.King, Rank.Queen, Rank.Six, Rank.Three, Rank.Two }, null)]
    public void EvaluateFiveCardHand(PokerHandType expectedHandType, string hand, Rank[] expectedPrimaryRanks, Rank[]? expectedSecondaryRanks)
    {
        var cardSet = new CardSet(CardFormat.Default.ParseMultipleOrThrow(hand));
        var pokerHand = new PokerEvaluator().EvaluateFiveCardHand(cardSet);

        pokerHand.Type.Should().Be(expectedHandType);
        pokerHand.PrimaryRanks.Should().BeEquivalentTo(expectedPrimaryRanks);
        pokerHand.SecondaryRanks.Should().BeEquivalentTo(expectedSecondaryRanks ?? Array.Empty<Rank>());
    }
    
    [Test]
    public void RankAddTest()
    {
        var hand = CardFormat.Default.ParseMultipleOrThrow("8D 8C 2D 2C KH");

        var rank = CalculateRankNumber(hand);
        var intrinsic = CalculateRankNumberIntrinsic2(hand);

        intrinsic.Should().Be(rank);
    }

    private static int CalculateRankNumber(IReadOnlyList<Card> hand)
    {
        var number = 0;
        foreach (var card in hand)
        {
            number *= 13;
            number += (int)card.Rank;
        }

        return number;
    }

    private static int CalculateRankNumberIntrinsic1(IReadOnlyList<Card> hand)
    {
        var vector = Vector128.Create((short)hand[0].Rank, (short)hand[1].Rank, (short)hand[2].Rank, (short)hand[3].Rank, (short)hand[4].Rank, 0, 0, 0);

        var multiply = Vector128.Create(
            Vector64.Create(13 * 13 * 13 * 13, 13 * 13 * 13, 13 * 13, 13),
            Vector64.Create(1, 0, 0, 0));

        var result1 = Sse2.MultiplyAddAdjacent(vector, multiply);

        var result2 = Ssse3.HorizontalAdd(result1, result1);
        var result3 = Ssse3.HorizontalAdd(result2, result2);

        return result3.ToScalar();
    }
    
    private static int CalculateRankNumberIntrinsic2(IReadOnlyList<Card> hand)
    {
        var vector = Vector128.Create((short)hand[0].Rank, (short)hand[1].Rank, (short)hand[2].Rank, (short)hand[3].Rank, 0, 0, 0, 0);

        var multiply = Vector128.Create(
            Vector64.Create(13 * 13 * 13 * 13, 13 * 13 * 13, 13 * 13, 13),
            Vector64.Create(0, 0, 0, 0));

        var result1 = Sse2.MultiplyAddAdjacent(vector, multiply);

        var result2 = Ssse3.HorizontalAdd(result1, result1);
        return result2.ToScalar() + (int)hand[4].Rank;
    }

    [Test]
    public void EvaluateFiveCardHand_AllHands()
    {
        var allFiveCardHands = Card.FullDeck.Combinations(5);

        var evaluator = new PokerEvaluator();
        var handsByType = CountHandTypes(allFiveCardHands, evaluator.EvaluateFiveCardHand);

        var expected = new Dictionary<PokerHandType, int>
        {
            { PokerHandType.FiveOfAKind, 0 },
            { PokerHandType.RoyalFlush, 4 },
            { PokerHandType.StraightFlush, 36 },
            { PokerHandType.FourOfAKind, 624 },
            { PokerHandType.FullHouse, 3744 },
            { PokerHandType.Flush, 5108 },
            { PokerHandType.Straight, 10200 },
            { PokerHandType.ThreeOfAKind, 54912 },
            { PokerHandType.TwoPair, 123552 },
            { PokerHandType.Pair, 1098240 },
            { PokerHandType.HighCard, 1302540 }
        };

        handsByType.Values.Sum().Should().Be(2598960);
        handsByType.Should().BeEquivalentTo(expected);
    }
    
    [TestCase("AS KS QS JS 10S 9S 2C 3C")]
    [TestCase("AS KS QS JS 2C 3C")]
    public void EvaluateSevenCardHand_ThrowsIfHandDoesNotHaveFiveCards(string hand)
    {
        var cardSet = new CardSet(CardFormat.Default.ParseMultipleOrThrow(hand));
        var evaluator = new PokerEvaluator();
        evaluator.Invoking(p => p.EvaluateSevenCardHand(cardSet)).Should().Throw<ArgumentException>();
    }
    
    // Taken from https://www.dummies.com/article/home-auto-hobbies/games/card-games/poker/playing-a-hand-of-seven-card-stud-poker-199748/.
    [TestCase(PokerHandType.FullHouse, "AS AH 6D 4C 4H 2H AC", new[] { Rank.Ace }, new[] { Rank.Four })]
    [TestCase(PokerHandType.Flush, "JD 4D 10D 9D KS 3H AD", new[] { Rank.Ace, Rank.Jack, Rank.Ten, Rank.Nine, Rank.Four }, null)]
    [TestCase(PokerHandType.TwoPair, "9S 7S 8H 8C 6C 6H 9D", new[] { Rank.Nine,  Rank.Eight }, new[] { Rank.Seven })]
    [TestCase(PokerHandType.FullHouse, "QC JS QS 2S QH JH 3D", new[] { Rank.Queen }, new[] { Rank.Jack })]
    [TestCase(PokerHandType.ThreeOfAKind, "5S 5D 5H KH 8D 7H 9C", new[] { Rank.Five }, new[] { Rank.King, Rank.Nine })]
    [TestCase(PokerHandType.Straight, "QD JC 10S 9H KC 4S 6S", new[] { Rank.King, Rank.Queen, Rank.Jack, Rank.Ten, Rank.Nine }, null)]
    public void EvaluateSevenCardHand(PokerHandType expectedHandType, string hand, Rank[] expectedPrimaryRanks, Rank[]? expectedSecondaryRanks)
    {
        var cardSet = new CardSet(CardFormat.Default.ParseMultipleOrThrow(hand));
        var pokerHand = new PokerEvaluator().EvaluateSevenCardHand(cardSet);

        pokerHand.Type.Should().Be(expectedHandType);
        pokerHand.PrimaryRanks.Should().BeEquivalentTo(expectedPrimaryRanks);
        pokerHand.SecondaryRanks.Should().BeEquivalentTo(expectedSecondaryRanks ?? Array.Empty<Rank>());
    }
    
    [Test]
    [Ignore("Takes several minutes to run.")]
    public void EvaluateSevenCardHand_AllHands()
    {
        var allSevenCardHands = Card.FullDeck.Combinations(7);
        
        var evaluator = new PokerEvaluator();
        var handsByType = CountHandTypes(allSevenCardHands, evaluator.EvaluateSevenCardHand);
        
        // http://people.math.sfu.ca/~alspach/comp20/
        var expected = new Dictionary<PokerHandType, int>
        {
            { PokerHandType.FiveOfAKind, 0 },
            { PokerHandType.RoyalFlush, 4324 },
            { PokerHandType.StraightFlush, 37260 },
            { PokerHandType.FourOfAKind, 224848 },
            { PokerHandType.FullHouse, 3473184 },
            { PokerHandType.Flush, 4047644 },
            { PokerHandType.Straight, 6180020 },
            { PokerHandType.ThreeOfAKind, 6461620 },
            { PokerHandType.TwoPair, 31433400 },
            { PokerHandType.Pair, 58627800 },
            { PokerHandType.HighCard, 23294460 }
        };

        handsByType.Values.Sum().Should().Be(133784560);
        handsByType.Should().BeEquivalentTo(expected);
    }

    [JetBrains.Annotations.MustUseReturnValue]
    private static IReadOnlyDictionary<PokerHandType, int> CountHandTypes([JetBrains.Annotations.InstantHandle] IEnumerable<IReadOnlyCardSet> hands, Func<IReadOnlyCardSet, PokerHand> evaluator)
    {
        var results = new int[Enum.GetValues<PokerHandType>().Length];
        foreach (var hand in hands.AsParallel().Select(evaluator))
        {
            Interlocked.Increment(ref results[(int)hand.Type]);
        }

        return results.Select((value, index) => (Type: (PokerHandType)index, Count: value)).ToDictionary(x => x.Type, x => x.Count);
    }
}