using System.Collections;
using System.Diagnostics.Contracts;
using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public abstract class ReadOnlyCardSetTestFixture<T>
    where T : IReadOnlyCardSet
{
    [Pure]
    protected abstract T Create(params Card[] cards);
    
    [Test]
    public void Contains()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.Contains(new Card(Rank.Ten, Suit.Spades)).Should().BeTrue();
        set.Contains(new Card(Rank.Four, Suit.Hearts)).Should().BeTrue();
        set.Contains(new Card(Rank.Eight, Suit.Diamonds)).Should().BeFalse();
        set.Contains(new Card(Rank.Queen, Suit.Clubs)).Should().BeFalse();
    }

    [Test]
    public void Count()
    {
        Create().Count.Should().Be(0);
        Create(new Card(Rank.Four, Suit.Hearts)).Count.Should().Be(1);
        Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Spades)).Count.Should().Be(2);
        Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ten, Suit.Spades), new Card(Rank.Queen, Suit.Clubs)).Count.Should().Be(3);
    }

    [Test]
    public void Enumerator_Current_ThrowsIfNotStarted()
    {
        using var enumerator = Create().GetEnumerator();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_NonGeneric_Current_ThrowsIfNotStarted()
    {
        IEnumerator enumerator = Create().GetEnumerator();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_Current_ThrowsIfFinished()
    {
        using var enumerator = Create().GetEnumerator();
        enumerator.MoveNext().Should().BeFalse();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_Current_NonGenericThrowsIfFinished()
    {
        IEnumerator enumerator = Create().GetEnumerator();
        enumerator.MoveNext().Should().BeFalse();
        enumerator.Invoking(e => e.Current).Should().Throw<InvalidOperationException>().WithMessage("Enumeration has either not started or has already finished.");
    }

    [Test]
    public void Enumerator_Reset()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
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
    public void IsProperSubsetOf_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsProperSubsetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeFalse();
        set.IsProperSubsetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeTrue();
        set.IsProperSubsetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
        set.IsProperSubsetOf(Create()).Should().BeFalse();

        Create().IsProperSubsetOf(Create()).Should().BeFalse();
        Create().IsProperSubsetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
    }

    [Test]
    public void IsProperSubsetOf_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeFalse();
        set.IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeTrue();
        set.IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
        set.IsProperSubsetOf(Array.Empty<Card>()).Should().BeFalse();

        Create().IsProperSubsetOf(Array.Empty<Card>()).Should().BeFalse();
        Create().IsProperSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
    }

    [Test]
    public void IsSubsetOf_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsSubsetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.IsSubsetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeTrue();
        set.IsSubsetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
        set.IsSubsetOf(Create()).Should().BeFalse();

        Create().IsSubsetOf(Create()).Should().BeTrue();
        Create().IsSubsetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
    }

    [Test]
    public void IsSubsetOf_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeTrue();
        set.IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeTrue();
        set.IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
        set.IsSubsetOf(Array.Empty<Card>()).Should().BeFalse();

        Create().IsSubsetOf(Array.Empty<Card>()).Should().BeTrue();
        Create().IsSubsetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
    }

    [Test]
    public void IsProperSupersetOf_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsProperSupersetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.IsProperSupersetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeFalse();
        set.IsProperSupersetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
        set.IsProperSupersetOf(Create()).Should().BeTrue();

        Create().IsProperSupersetOf(Create()).Should().BeFalse();
        Create().IsProperSupersetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
    }

    [Test]
    public void IsProperSupersetOf_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeTrue();
        set.IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeFalse();
        set.IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
        set.IsProperSupersetOf(Array.Empty<Card>()).Should().BeTrue();

        Create().IsProperSupersetOf(Array.Empty<Card>()).Should().BeFalse();
        Create().IsProperSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
    }

    [Test]
    public void IsSupersetOf_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsSupersetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.IsSupersetOf(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeTrue();
        set.IsSupersetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
        set.IsSupersetOf(Create()).Should().BeTrue();

        Create().IsSupersetOf(Create()).Should().BeTrue();
        Create().IsSupersetOf(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
    }

    [Test]
    public void IsSupersetOf_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs));
        set.IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) } ).Should().BeTrue();
        set.IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) } ).Should().BeTrue();
        set.IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeTrue();
        set.IsSupersetOf(Array.Empty<Card>()).Should().BeTrue();

        Create().IsSupersetOf(Array.Empty<Card>()).Should().BeTrue();
        Create().IsSupersetOf(new[] { new Card(Rank.Ten, Suit.Spades) } ).Should().BeFalse();
    }

    [Test]
    public void Overlaps_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        set.Overlaps(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeTrue();
        set.Overlaps(Create(new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Nine, Suit.Spades))).Should().BeTrue();
        set.Overlaps(Create(new Card(Rank.Nine, Suit.Spades))).Should().BeFalse();
        set.Overlaps(Create()).Should().BeFalse();

        Create().Overlaps(Create()).Should().BeFalse();
    }

    [Test]
    public void Overlaps_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Ten, Suit.Clubs));
        set.Overlaps(new[] { new Card(Rank.Ten, Suit.Spades) }).Should().BeTrue();
        set.Overlaps(new[] { new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Nine, Suit.Spades) }).Should().BeTrue();
        set.Overlaps(new[] { new Card(Rank.Nine, Suit.Spades) }).Should().BeFalse();
        set.Overlaps(Array.Empty<Card>()).Should().BeFalse();

        Create().Overlaps(Array.Empty<Card>()).Should().BeFalse();
    }

    [Test]
    public void SetEquals_IReadOnlyCardSet()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.SetEquals(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts))).Should().BeTrue();
        set.SetEquals(Create(new Card(Rank.Ten, Suit.Spades))).Should().BeFalse();
        set.SetEquals(Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs))).Should().BeFalse();
        set.SetEquals(Create()).Should().BeFalse();

        Create().SetEquals(Create()).Should().BeTrue();
    }

    [Test]
    public void SetEquals_IEnumerable()
    {
        var set = Create(new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts));
        set.SetEquals(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) }).Should().BeTrue();
        set.SetEquals(new[] { new Card(Rank.Ten, Suit.Spades) }).Should().BeFalse();
        set.SetEquals(new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts), new Card(Rank.Seven, Suit.Clubs) }).Should().BeFalse();
        set.SetEquals(Array.Empty<Card>()).Should().BeFalse();

        Create().SetEquals(Array.Empty<Card>()).Should().BeTrue();
    }
}