using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public abstract class MutableCardSetTestFixture<T> : ReadOnlyCardSetTestFixture<T>
    where T : ICardSet
{
    [Test]
    public void Add()
    {
        var set = Create();
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
        ICollection<Card> set = Create();
        set.Add(new Card(Rank.Four, Suit.Hearts));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts) });

        set.Add(new Card(Rank.Ten, Suit.Spades));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });

        set.Add(new Card(Rank.Four, Suit.Hearts));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });
    }

    [Test]
    public void Clear()
    {
        var set = Create(new Card(Rank.Four, Suit.Hearts));
        set.Clear().Should().BeTrue();
        set.Should().BeEmpty();

        set.Clear().Should().BeFalse();
        set.Should().BeEmpty();
    }

    [Test]
    public void Clear_ICollection()
    {
        ICollection<Card> set = Create(new Card(Rank.Four, Suit.Hearts));
        set.Clear();
        set.Should().BeEmpty();

        set.Clear();
        set.Should().BeEmpty();
    }

    [Test]
    public void CopyTo_DestinationTooSmall()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        var destination = new Card[1];
        set.Invoking(s => s.CopyTo(destination, 0)).Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void CopyTo_ArrayIndexTooLarge()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        var destination = new Card[2];
        set.Invoking(s => s.CopyTo(destination, 1)).Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void CopyTo()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        var destination = new Card[4];
        set.CopyTo(destination, 1);
        destination.Should().BeEquivalentTo(new Card[] { default, new (Rank.Ten, Suit.Spades), new (Rank.Four, Suit.Hearts), default });
    }

    [Test]
    public void IsReadOnly() => Create().IsReadOnly.Should().BeFalse();

    [Test]
    public void Remove()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.Remove(new Card(Rank.Four, Suit.Hearts)).Should().BeTrue();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });

        set.Remove(new Card(Rank.Seven, Suit.Clubs)).Should().BeTrue();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });

        set.Remove(new Card(Rank.Ace, Suit.Spades)).Should().BeFalse();
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });
    }

    [Test]
    public void ExceptWith_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IReadOnlyCardSet toRemove = Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds));
        set.ExceptWith(toRemove);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void ExceptWith_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toRemove = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        set.ExceptWith(toRemove);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void IntersectWith_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IReadOnlyCardSet toIntersect = Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds));
        set.IntersectWith(toIntersect);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
    }

    [Test]
    public void IntersectWith_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toIntersect = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        set.IntersectWith(toIntersect);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
    }

    [Test]
    public void SymmetricExceptWith_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IReadOnlyCardSet toAdd = Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.SymmetricExceptWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void SymmetricExceptWith_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        set.SymmetricExceptWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void UnionWith_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IReadOnlyCardSet toAdd = Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.UnionWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void UnionWith_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        set.UnionWith(toAdd);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
    }
}