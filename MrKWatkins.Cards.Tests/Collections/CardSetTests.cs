using System.Collections;
using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable CollectionNeverUpdated.Local

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class CardSetTests
{
    [Test]
    public void Constructor_Parameterless() => new CardSet().Should().BeEmpty();

    [Test]
    public void Constructor_Params()
    {
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
    public void Add()
    {
        var set = new CardSet();
        set.Add(new Card(Rank.Four, Suit.Hearts)).Should().BeTrue();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts) });

        set.Add(new Card(Rank.Ten, Suit.Spades)).Should().BeTrue();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });

        set.Add(new Card(Rank.Four, Suit.Hearts)).Should().BeFalse();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });
    }

    [Test]
    public void Add_ICollection()
    {
        ICollection<Card> set = new CardSet();
        set.Add(new Card(Rank.Four, Suit.Hearts));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts) });

        set.Add(new Card(Rank.Ten, Suit.Spades));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });

        set.Add(new Card(Rank.Four, Suit.Hearts));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });
    }

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
    public void Clear()
    {
        var set = new CardSet(new Card(Rank.Four, Suit.Hearts));
        set.Clear().Should().BeTrue();
        set.Should().BeEmpty();

        set.Clear().Should().BeFalse();
        set.Should().BeEmpty();
    }

    [Test]
    public void Clear_ICollection()
    {
        ICollection<Card> set = new CardSet(new Card(Rank.Four, Suit.Hearts));
        set.Clear();
        set.Should().BeEmpty();

        set.Clear();
        set.Should().BeEmpty();
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
    public void Contains()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.Contains(new Card(Rank.Ten, Suit.Spades)).Should().BeTrue();
        set.Contains(new Card(Rank.Four, Suit.Hearts)).Should().BeTrue();
        set.Contains(new Card(Rank.Eight, Suit.Diamonds)).Should().BeFalse();
        set.Contains(new Card(Rank.Queen, Suit.Clubs)).Should().BeFalse();
    }

    [Test]
    public void CopyTo_DestinationTooSmall()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        var destination = new Card[1];
        set.Invoking(s => s.CopyTo(destination, 0)).Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void CopyTo_ArrayIndexTooLarge()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        var destination = new Card[2];
        set.Invoking(s => s.CopyTo(destination, 1)).Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void CopyTo()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        var destination = new Card[4];
        set.CopyTo(destination, 1);
        destination.Should().BeEquivalentTo(new Card[] { default, new (Rank.Ten, Suit.Spades), new (Rank.Four, Suit.Hearts), default });
    }

    [Test]
    public void Count()
    {
        var set = new CardSet();
        set.Count.Should().Be(0);

        set.Add(new Card(Rank.Four, Suit.Hearts));
        set.Count.Should().Be(1);

        set.Add(new Card(Rank.Ten, Suit.Spades));
        set.Count.Should().Be(2);

        set.Add(new Card(Rank.Queen, Suit.Clubs));
        set.Count.Should().Be(3);
    }

    [Test]
    public void IsReadOnly() => new CardSet().IsReadOnly.Should().BeFalse();

    [Test]
    public void Remove()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.Remove(new Card(Rank.Four, Suit.Hearts)).Should().BeTrue();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });

        set.Remove(new Card(Rank.Seven, Suit.Clubs)).Should().BeTrue();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });

        set.Remove(new Card(Rank.Ace, Suit.Spades)).Should().BeFalse();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });
    }

    [Test]
    public void Remove_ModifiesVersionIfSetChanges()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        AssertModifiesVersion(set, s => s.Remove(new Card(Rank.Four, Suit.Hearts)));
        AssertDoesNotModifyVersion(set, s => s.Remove(new Card(Rank.Four, Suit.Hearts)));
    }

    [Test]
    public void Enumerator_Current_ThrowsIfNotStarted()
    {
        using var enumerator = new CardSet().GetEnumerator();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_NonGeneric_Current_ThrowsIfNotStarted()
    {
        IEnumerator enumerator = new CardSet().GetEnumerator();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_Current_ThrowsIfFinished()
    {
        using var enumerator = new CardSet().GetEnumerator();
        enumerator.MoveNext().Should().BeFalse();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_Current_NonGenericThrowsIfFinished()
    {
        IEnumerator enumerator = new CardSet().GetEnumerator();
        enumerator.MoveNext().Should().BeFalse();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_Reset()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        IEnumerator enumerator = set.GetEnumerator();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be(new Card(Rank.Four, Suit.Hearts));
        
        enumerator.Reset();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be(new Card(Rank.Four, Suit.Hearts));
    }

    [Test]
    public void ExceptWith_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IReadOnlyCardSet toRemove = new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds));
        set.ExceptWith(toRemove);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void ExceptWith_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toRemove = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        set.ExceptWith(toRemove);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
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
    public void IntersectWith_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IReadOnlyCardSet toIntersect = new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds));
        set.IntersectWith(toIntersect);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
    }

    [Test]
    public void IntersectWith_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toIntersect = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        set.IntersectWith(toIntersect);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
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
    public void IsProperSubsetOf_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsProperSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeFalse();
        set.IsProperSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeTrue();
        set.IsProperSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
        set.IsProperSubsetOf(new CardSet()).Should().BeFalse();

        new CardSet().IsProperSubsetOf(new CardSet()).Should().BeFalse();
        new CardSet().IsProperSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
    }

    [Test]
    public void IsProperSubsetOf_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeFalse();
        set.IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeTrue();
        set.IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
        set.IsProperSubsetOf(Array.Empty<Card>()).Should().BeFalse();

        new CardSet().IsProperSubsetOf(Array.Empty<Card>()).Should().BeFalse();
        new CardSet().IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
    }

    [Test]
    public void IsSubsetOf_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.IsSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeTrue();
        set.IsSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
        set.IsSubsetOf(new CardSet()).Should().BeFalse();

        new CardSet().IsSubsetOf(new CardSet()).Should().BeTrue();
        new CardSet().IsSubsetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
    }

    [Test]
    public void IsSubsetOf_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeTrue();
        set.IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeTrue();
        set.IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
        set.IsSubsetOf(Array.Empty<Card>()).Should().BeFalse();

        new CardSet().IsSubsetOf(Array.Empty<Card>()).Should().BeTrue();
        new CardSet().IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
    }

    [Test]
    public void IsProperSupersetOf_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsProperSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.IsProperSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeFalse();
        set.IsProperSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
        set.IsProperSupersetOf(new CardSet()).Should().BeTrue();

        new CardSet().IsProperSupersetOf(new CardSet()).Should().BeFalse();
        new CardSet().IsProperSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
    }

    [Test]
    public void IsProperSupersetOf_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeTrue();
        set.IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeFalse();
        set.IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
        set.IsProperSupersetOf(Array.Empty<Card>()).Should().BeTrue();

        new CardSet().IsProperSupersetOf(Array.Empty<Card>()).Should().BeFalse();
        new CardSet().IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
    }

    [Test]
    public void IsSupersetOf_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.IsSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeTrue();
        set.IsSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
        set.IsSupersetOf(new CardSet()).Should().BeTrue();

        new CardSet().IsSupersetOf(new CardSet()).Should().BeTrue();
        new CardSet().IsSupersetOf(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
    }

    [Test]
    public void IsSupersetOf_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeTrue();
        set.IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeTrue();
        set.IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
        set.IsSupersetOf(Array.Empty<Card>()).Should().BeTrue();

        new CardSet().IsSupersetOf(Array.Empty<Card>()).Should().BeTrue();
        new CardSet().IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
    }

    [Test]
    public void Overlaps_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        set.Overlaps(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
        set.Overlaps(new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Nine, Suit.Spades))).Should().BeTrue();
        set.Overlaps(new CardSet(new Card(Rank.Nine, Suit.Spades))).Should().BeFalse();
        set.Overlaps(new CardSet()).Should().BeFalse();

        new CardSet().Overlaps(new CardSet()).Should().BeFalse();
    }

    [Test]
    public void Overlaps_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        set.Overlaps(new[] { new Card(Rank.Ten, Suit.Spades) }).Should().BeTrue();
        set.Overlaps(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Nine, Suit.Spades) }).Should().BeTrue();
        set.Overlaps(new[] { new Card(Rank.Nine, Suit.Spades) }).Should().BeFalse();
        set.Overlaps(Array.Empty<Card>()).Should().BeFalse();

        new CardSet().Overlaps(Array.Empty<Card>()).Should().BeFalse();
    }

    [Test]
    public void SetEquals_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.SetEquals(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.SetEquals(new CardSet(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
        set.SetEquals(new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeFalse();
        set.SetEquals(new CardSet()).Should().BeFalse();

        new CardSet().SetEquals(new CardSet()).Should().BeTrue();
    }

    [Test]
    public void SetEquals_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.SetEquals(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) }).Should().BeTrue();
        set.SetEquals(new[] { new Card(Rank.Ten, Suit.Spades) }).Should().BeFalse();
        set.SetEquals(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) }).Should().BeFalse();
        set.SetEquals(Array.Empty<Card>()).Should().BeFalse();

        new CardSet().SetEquals(Array.Empty<Card>()).Should().BeTrue();
    }

    [Test]
    public void SymmetricExceptWith_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IReadOnlyCardSet toAdd = new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.SymmetricExceptWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void SymmetricExceptWith_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        set.SymmetricExceptWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
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
    public void UnionWith_IReadOnlyCardSet()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IReadOnlyCardSet toAdd = new CardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.UnionWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void UnionWith_IEnumerable()
    {
        var set = new CardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        set.UnionWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
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