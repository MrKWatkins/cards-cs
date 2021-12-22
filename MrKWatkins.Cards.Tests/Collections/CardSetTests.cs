using System.Collections;
using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class CardSetTests : MutableCardSetTestFixture<CardSet>
{
    protected override CardSet Create(params Card[] cards) => new (cards);
    
    [Test]
    public void Constructor_Parameterless() => new CardSet().Should().BeEmpty();

    [Test]
    public void Constructor_Params()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var set = new CardSet(new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs) });
    }

    [Test]
    public void Constructor_IEnumerable()
    {
        IEnumerable<Card> cards = new[] { new Card(Rank.Two, Suit.Hearts), new Card(Rank.Ten, Suit.Diamonds), new Card(Rank.Queen, Suit.Spades) };
        var set = new CardSet(cards);
        set.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void Constructor_IEnumerable_ActuallyIReadOnlyCardSet()
    {
        // To check the short cut in ToBitIndices.
        IEnumerable<Card> cards = new CardSet(new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs));
        var set = new CardSet(cards);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs) });
    }

    [Test]
    public void Constructor_IReadOnlyCardSet()
    {
        IReadOnlyCardSet original = new CardSet(new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Five, Suit.Hearts), new Card(Rank.Four, Suit.Clubs));
        var set = new CardSet(original);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Five, Suit.Hearts), new Card(Rank.Four, Suit.Clubs) });
    }

    [Test]
    public void CreateFullDeck() => CardSet.CreateFullDeck().Should().BeEquivalentTo(Card.FullDeck(), c => c.WithStrictOrdering());

    [Test]
    public void Add_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet();
        AssertModifiesVersion(set, s => s.Add(new Card(Rank.Four, Suit.Hearts)));
        AssertDoesNotModifyVersion(set, s => s.Add(new Card(Rank.Four, Suit.Hearts)));
    }

    [Test]
    public void Add_ICollection_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet();
        AssertModifiesVersion(set, s => ((ICollection<Card>)s).Add(new Card(Rank.Four, Suit.Hearts)));
        AssertDoesNotModifyVersion(set, s => ((ICollection<Card>)s).Add(new Card(Rank.Four, Suit.Hearts)));
    }

    [Test]
    public void Clear_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Four, Suit.Hearts));
        AssertModifiesVersion(set, s => s.Clear());
        AssertDoesNotModifyVersion(set, s => s.Clear());
    }

    [Test]
    public void Clear_ICollection_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Four, Suit.Hearts));
        AssertModifiesVersion(set, s => ((ICollection<Card>)s).Clear());
        AssertDoesNotModifyVersion(set, s => ((ICollection<Card>)s).Clear());
    }

    [Test]
    public void Remove_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        AssertModifiesVersion(set, s => s.Remove(new Card(Rank.Four, Suit.Hearts)));
        AssertDoesNotModifyVersion(set, s => s.Remove(new Card(Rank.Four, Suit.Hearts)));
    }

    [Test]
    public void ExceptWith_IReadOnlyCardSet_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        AssertModifiesVersion(set, s => s.ExceptWith(new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds))));
        AssertDoesNotModifyVersion(set, s => s.ExceptWith(new CardSet(new Card(Rank.King, Suit.Clubs))));
    }

    [Test]
    public void ExceptWith_IEnumerable_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        AssertModifiesVersion(set, s => s.ExceptWith(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) }));
        AssertDoesNotModifyVersion(set, s => s.ExceptWith(new[] { new Card(Rank.King, Suit.Clubs) }));
    }

    [Test]
    public void IntersectWith_IReadOnlyCardSet_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        AssertModifiesVersion(set, s => s.IntersectWith(new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds))));
        AssertDoesNotModifyVersion(set, s => s.IntersectWith(new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs))));
    }

    [Test]
    public void IntersectWith_IEnumerable_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        AssertModifiesVersion(set, s => s.IntersectWith(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) }));
        AssertDoesNotModifyVersion(set, s => s.IntersectWith(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) }));
    }

    [Test]
    public void SymmetricExceptWith_IReadOnlyCardSet_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        AssertModifiesVersion(set, s => s.SymmetricExceptWith(new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))));
        AssertDoesNotModifyVersion(set, s => s.SymmetricExceptWith(new CardSet()));
    }

    [Test]
    public void SymmetricExceptWith_IEnumerable_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        AssertModifiesVersion(set, s => s.SymmetricExceptWith(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) }));
        AssertDoesNotModifyVersion(set, s => s.SymmetricExceptWith(Array.Empty<Card>()));
    }

    [Test]
    public void UnionWith_IReadOnlyCardSet_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        AssertModifiesVersion(set, s => s.UnionWith(new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))));
        AssertDoesNotModifyVersion(set, s => s.UnionWith(new CardSet(new Card(Rank.Four, Suit.Hearts))));
    }

    [Test]
    public void UnionWith_IEnumerable_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        AssertModifiesVersion(set, s => s.UnionWith(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) }));
        AssertDoesNotModifyVersion(set, s => s.UnionWith(new[] { new Card(Rank.Four, Suit.Hearts) }));
    }

    private static void AssertModifiesVersion(CardSet set, Action<CardSet> action)
    {
        IEnumerator enumerator = set.GetEnumerator();
        action(set);
        enumerator.Invoking(e => e.MoveNext()).Should().Throw<InvalidOperationException>().WithMessage("Collection was modified; enumeration operation may not execute.");
        enumerator.Invoking(e => e.Reset()).Should().Throw<InvalidOperationException>().WithMessage("Collection was modified; enumeration operation may not execute.");
    }

    private static void AssertDoesNotModifyVersion(CardSet set, Action<CardSet> action)
    {
        IEnumerator enumerator = set.GetEnumerator();
        action(set);
        enumerator.Invoking(e => e.MoveNext()).Should().NotThrow();
        enumerator.Invoking(e => e.Reset()).Should().NotThrow();
    }
}