using System.Diagnostics.Contracts;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Testing;
using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Poker.Tests;

public sealed class PokerHandTests
{
    [TestCaseSource(nameof(EqualityTestCases))]
    public void Equality(PokerHand x, PokerHand y, bool expectedEqual) => EqualityTests.AssertEqual(x, y, expectedEqual);

    [Pure]
    public static IEnumerable<TestCaseData> EqualityTestCases() => EqualityTests.CreateTestData(CreateOrderedPokerHands);

    [TestCaseSource(nameof(Equality_DifferentSuitsTestCases))]
    public void Equality_DifferentSuits(PokerHand x, PokerHand y, bool expectedEqual) => EqualityTests.AssertEqual(x, y, expectedEqual);

    [Pure]
    public static IEnumerable<TestCaseData> Equality_DifferentSuitsTestCases() => 
        EqualityTests.CreateTestData(CreateOrderedPokerHands, CreateDifferentSuitOrderedPokerHands);

    [TestCaseSource(nameof(ComparableTestCases))]
    public void Comparable(PokerHand x, PokerHand y, int expectedSign) => ComparableTests.AssertCompareTo(x, y, expectedSign);

    [Pure]
    public static IEnumerable<TestCaseData> ComparableTestCases() => ComparableTests.CreateTestData(CreateOrderedPokerHands);

    [Pure]
    private static IEnumerable<PokerHand> CreateOrderedPokerHands() => OrderedPokerHands.Select(CreatePokerHand);

    [Pure]
    private static IEnumerable<PokerHand> CreateDifferentSuitOrderedPokerHands() => OrderedPokerHands.Select(h => CreatePokerHand(RotateSuits(h)));

    private static readonly IReadOnlyList<string> OrderedPokerHands = new[]
    {
        // High card.
        "7S 6S 5S 4C 2S",
        "KS QS 6H 3C 2C",
        "AS KS QS JS 9C",

        // Pair.
        "7H 7C KC QS JS",
        "7H 7C AC KS JS",
        "8H 8C AC KS JS",
        "AS AC KH QS JS",

        // Two pair.
        "7H 7C 5C 5S JS",
        "7H 7C 6C 6S JS",
        "8H 8C 6C 6S JS",
        "AH AC 6C 6S JS",

        // Three of a kind. 
        "9S 9D 9C 3H 2D",
        "9S 9D 9C 4H 2D",
        "10S 10H 10C AC 9D",
        "AS AH AC KC 9D",

        // Straight.
        "5C 4C 3C 2S AC",
        "9D 8D 7D 6C 5D",
        "AD KS QS JS 10H",

        // Flush.
        "8C 7C 4C 3C 2C",
        "KS QS JS 9S 8S",
        "AH JH 7H 4H 2H",

        // Full house.
        "4S 4H 4D 8S 8H",
        "KS KH KC QD QH",
        "KS KH KC AD AH",
        "AS AH AC QD QH",
        "AS AH AC KD KH",

        // Four of a kind.
        "4S 4H 4D 4C 7H",
        "AS AH AD AC QH",
        "AS AH AD AC KH",

        // Straight flush.
        "5C 4C 3C 2C AC",
        "9D 8D 7D 6D 5D",

        // Royal flush.
        "AS KS QS JS 10S",
    };
    
    [Pure]
    private static PokerHand CreatePokerHand(string hand)
    {
        var cardSet = new CardSet(CardFormat.Default.ParseMultipleOrThrow(hand));
        return new PokerEvaluator().EvaluateFiveCardHand(cardSet);
    }

    [Pure]
    private static string RotateSuits(string hand) =>
        string.Create(hand.Length, hand, (output, h) =>
        {
            for (var f = 0; f < h.Length; f++)
            {
                output[f] = hand[f] switch
                {
                    'S' => 'H',
                    'H' => 'D',
                    'D' => 'C',
                    'C' => 'S',
                    var other => other
                };
            }
        });
}