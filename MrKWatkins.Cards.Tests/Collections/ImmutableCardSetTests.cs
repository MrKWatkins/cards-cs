using System.Collections.Immutable;
using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class ImmutableCardSetTests : ReadOnlyCardSetTestFixture<ImmutableCardSet>
{
    protected override ImmutableCardSet Create(params Card[] cards) => new (cards);
    
    [Test]
    public void Constructor_Parameterless() => new ImmutableCardSet().Should().BeEmpty();

    [Test]
    public void Constructor_Params()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs));
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs) });
    }

    [Test]
    public void Constructor_IEnumerable()
    {
        IEnumerable<Card> cards = new[] { new Card(Rank.Two, Suit.Hearts), new Card(Rank.Ten, Suit.Diamonds), new Card(Rank.Queen, Suit.Spades) };
        var set = new ImmutableCardSet(cards);
        set.Should().BeEquivalentTo(cards);
    }

    [Test]
    public void Constructor_IEnumerable_ActuallyIReadOnlyCardSet()
    {
        // To check the short cut in ToBitIndices.
        IEnumerable<Card> cards = new ImmutableCardSet(new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs));
        var set = new ImmutableCardSet(cards);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Ace, Suit.Spades), new Card(Rank.Eight, Suit.Hearts), new Card(Rank.King, Suit.Clubs) });
    }

    [Test]
    public void Constructor_IReadOnlyCardSet()
    {
        IReadOnlyCardSet original = new ImmutableCardSet(new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Five, Suit.Hearts), new Card(Rank.Four, Suit.Clubs));
        var set = new ImmutableCardSet(original);
        set.Should().BeEquivalentTo(new[] { new Card(Rank.Queen, Suit.Hearts), new Card(Rank.Five, Suit.Hearts), new Card(Rank.Four, Suit.Clubs) });
    }

    [Test]
    public void Empty() => ImmutableCardSet.Empty.Should().BeEmpty();

    [Test]
    public void FullDeck() => ImmutableCardSet.FullDeck.Should().BeEquivalentTo(Card.FullDeck, c => c.WithStrictOrdering());

    [Test]
    public void Add()
    {
        var set1 = new ImmutableCardSet();
        var set2 = set1.Add(new Card(Rank.Four, Suit.Hearts));
        set2.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts) });

        var set3 = set2.Add(new Card(Rank.Ten, Suit.Spades));
        set3.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });

        var set4 = set3.Add(new Card(Rank.Four, Suit.Hearts));
        set4.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });
    }
    
    [Test]
    public void Add_IImmutableSetInterface()
    {
        IImmutableSet<Card> set1 = new ImmutableCardSet();
        var set2 = set1.Add(new Card(Rank.Four, Suit.Hearts));
        set2.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts) });

        var set3 = set2.Add(new Card(Rank.Ten, Suit.Spades));
        set3.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });

        var set4 = set3.Add(new Card(Rank.Four, Suit.Hearts));
        set4.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) });
    }

    [Test]
    public void Clear()
    {
        var set1 = new ImmutableCardSet(new Card(Rank.Four, Suit.Hearts));
        var set2 = set1.Clear();
        set2.Should().BeEmpty();

        var set3 = set2.Clear();
        set3.Should().BeEmpty();
    }
    
    [Test]
    public void Clear_IImmutableSetInterface()
    {
        IImmutableSet<Card> set1 = new ImmutableCardSet(new Card(Rank.Four, Suit.Hearts));
        var set2 = set1.Clear();
        set2.Should().BeEmpty();

        var set3 = set2.Clear();
        set3.Should().BeEmpty();
    }

    [Test]
    public void Remove()
    {
        var set1 = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        var set2 = set1.Remove(new Card(Rank.Four, Suit.Hearts));
        set2.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });

        var set3 = set2.Remove(new Card(Rank.Seven, Suit.Clubs));
        set3.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });

        var set4 = set3.Remove(new Card(Rank.Ace, Suit.Spades));
        set4.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });
    }
    
    [Test]
    public void Remove_IImmutableSetInterface()
    {
        IImmutableSet<Card> set1 = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        var set2 = set1.Remove(new Card(Rank.Four, Suit.Hearts));
        set2.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });

        var set3 = set2.Remove(new Card(Rank.Seven, Suit.Clubs));
        set3.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });

        var set4 = set3.Remove(new Card(Rank.Ace, Suit.Spades));
        set4.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades) });
    }

    [Test]
    public void Except_IReadOnlyCardSet()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IReadOnlyCardSet toRemove = new ImmutableCardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds));
        var except = set.Except(toRemove);
        except.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void Except_IEnumerable()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toRemove = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        var except = set.Except(toRemove);
        except.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }
    
    [Test]
    public void Except_IImmutableSetInterface()
    {
        IImmutableSet<Card> set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toRemove = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        var except = set.Except(toRemove);
        except.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void Intersect_IReadOnlyCardSet()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IReadOnlyCardSet toIntersect = new ImmutableCardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds));
        var intersect = set.Intersect(toIntersect);
        intersect.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
    }

    [Test]
    public void Intersect_IEnumerable()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toIntersect = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        var intersect = set.Intersect(toIntersect);
        intersect.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
    }

    [Test]
    public void Intersect_IImmutableSetInterface()
    {
        IImmutableSet<Card> set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        IEnumerable<Card> toIntersect = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ace, Suit.Diamonds) };
        var intersect = set.Intersect(toIntersect);
        intersect.Should().BeEquivalentTo(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Clubs) });
    }

    [Test]
    public void SymmetricExcept_IReadOnlyCardSet()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IReadOnlyCardSet toAdd = new ImmutableCardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        var symmetricExcept = set.SymmetricExcept(toAdd);
        symmetricExcept.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void SymmetricExcept_IEnumerable()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        var symmetricExcept = set.SymmetricExcept(toAdd);
        symmetricExcept.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void SymmetricExcept_IImmutableSetInterface()
    {
        IImmutableSet<Card> set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        var symmetricExcept = set.SymmetricExcept(toAdd);
        symmetricExcept.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void TryGetValue()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.TryGetValue(new Card(Rank.Four, Suit.Hearts), out var got).Should().BeTrue();
        got.Should().Be(new Card(Rank.Four, Suit.Hearts));
        
        set.TryGetValue(new Card(Rank.Nine, Suit.Hearts), out _).Should().BeFalse();
    }

    [Test]
    public void Union_IReadOnlyCardSet()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IReadOnlyCardSet toAdd = new ImmutableCardSet(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        var union = set.Union(toAdd);
        union.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void Union_IEnumerable()
    {
        var set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        var union = set.Union(toAdd);
        union.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
    }

    [Test]
    public void Union__IImmutableSetInterface()
    {
        IImmutableSet<Card> set = new ImmutableCardSet(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        IEnumerable<Card> toAdd = new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) };
        var union = set.Union(toAdd);
        union.Should().BeEquivalentTo(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) });
    }
}