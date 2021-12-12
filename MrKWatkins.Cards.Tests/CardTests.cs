using FluentAssertions;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests;

// TODO: Equality tests.
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
    public void ToString_Test() => new Card(Rank.Ten, Suit.Clubs).ToString().Should().Be("10C");
}