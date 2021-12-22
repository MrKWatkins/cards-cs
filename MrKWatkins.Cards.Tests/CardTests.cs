using System.Diagnostics.Contracts;
using FluentAssertions;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests;

public sealed class CardTests
{
    [Test]
    public void Constructor()
    {
        var card = new Card(Rank.Five, Suit.Clubs);
        card.Rank.Should().Be(Rank.Five);
        card.Suit.Should().Be(Suit.Clubs);
    }

    [Test]
    public void Ranks() => 
        Card.Ranks.Should().BeEquivalentTo(
            new[] { Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King },
            c => c.WithStrictOrdering());

    [Test]
    public void Suits() => 
        Card.Suits.Should().BeEquivalentTo(
            new[] { Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs },
            c => c.WithStrictOrdering());

    [Test]
    public void FullDeck() => Card.FullDeck().Should().OnlyHaveUniqueItems().And.HaveCount(52);

    [Test]
    public void Index() => Card.FullDeck().Select(c => c.Index).Should().BeEquivalentTo(Enumerable.Range(0, 52));

    [Test]
    public void FromIndex() => Card.FullDeck().Select(c => Card.FromIndex(c.Index)).Should().BeEquivalentTo(Card.FullDeck());

    [Test]
    public void BitIndex() => Card.FullDeck().Select(c => c.BitIndex).Should().BeEquivalentTo(Enumerable.Range(0, 52).Select(i => 1UL << i));
    
    [Test]
    public void FromBitIndex() => Card.FullDeck().Select(c => Card.FromBitIndex(c.BitIndex)).Should().BeEquivalentTo(Card.FullDeck());

    [Test]
    public void ToString_Test() => new Card(Rank.Ten, Suit.Clubs).ToString().Should().Be("10C");

    [TestCaseSource(nameof(EqualityTestCases))]
    public void Equality(Card x, Card y, bool expectedEqual) => EqualityTests.AssertEqual(x, y, expectedEqual);

    [Pure]
    public static IEnumerable<TestCaseData> EqualityTestCases()
    {
        var fullDeck = Card.FullDeck().ToList();
        for (var f = 0; f < 52; f++)
        {
            for (var g = 0; g < 52; g++)
            {
                yield return new TestCaseData(fullDeck[f], fullDeck[g], f == g);
            }
        }
    }
}