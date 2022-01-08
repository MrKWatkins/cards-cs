using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class ConcurrentCardSetTests : MutableCardSetTestFixture<ConcurrentCardSet>
{
    protected override ConcurrentCardSet Create(params Card[] cards) => new (cards);
    
    [Test]
    public void Constructor_Parameterless() => new ConcurrentCardSet().Should().BeEmpty();

    [Test]
    public void Constructor_Params()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var set = new ConcurrentCardSet(new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs) });
    }

    [Test]
    public void Constructor_IEnumerable()
    {
        IEnumerable<Card> cards = new[] { new Card(Rank.Two, Suit.Hearts), new Card(Rank.Ten, Suit.Diamonds), new Card(Rank.Queen, Suit.Spades) };
        var set = new ConcurrentCardSet(cards);
        set.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void Constructor_IEnumerable_ActuallyIReadOnlyCardSet()
    {
        // To check the short cut in ToBitIndices.
        IEnumerable<Card> cards = new ConcurrentCardSet(new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs));
        var set = new ConcurrentCardSet(cards);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs) });
    }

    [Test]
    public void Constructor_IReadOnlyCardSet()
    {
        IReadOnlyCardSet original = new ConcurrentCardSet(new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Five, Suit.Hearts), new Card(Rank.Four, Suit.Clubs));
        var set = new ConcurrentCardSet(original);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Five, Suit.Hearts), new Card(Rank.Four, Suit.Clubs) });
    }

    [Test]
    public void CreateFullDeck() => ConcurrentCardSet.CreateFullDeck().Should().BeEquivalentTo(Card.FullDeck, c => c.WithStrictOrdering());
}