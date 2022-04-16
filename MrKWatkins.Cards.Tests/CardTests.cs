using System.Diagnostics.Contracts;
using System.Runtime.Intrinsics.X86;
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
    public void FullDeck() => Card.FullDeck.Should().OnlyHaveUniqueItems().And.HaveCount(52);

    [Test]
    public void Index() => Card.FullDeck.Select(c => c.Index).Should().BeEquivalentTo(Enumerable.Range(0, 52));

    [Test]
    public void FromIndex() => Card.FullDeck.Select(c => Card.FromIndex(c.Index)).Should().BeEquivalentTo(Card.FullDeck);

    [Test]
    public void BitIndex() => Card.FullDeck.Select(c => c.BitIndex).Should().BeEquivalentTo(Enumerable.Range(0, 52).Select(i => 1UL << i));
    
    [Test]
    public void FromBitIndex() => Card.FullDeck.Select(c => Card.FromBitIndex(c.BitIndex)).Should().BeEquivalentTo(Card.FullDeck);

    [Test]
    public void BitMask()
    {
        for (var suit = 0; suit < 4; suit++)
        {
            for (var rank = 0; rank < 13; rank++)
            {
                var card = new Card((Rank) rank, (Suit) suit);
                card.BitMask.Should().Be(1UL << rank << (suit * 16));
            }
        }
    }
    
    [Test]
    public void AceHighBitMask()
    {
        for (var suit = 0; suit < 4; suit++)
        {
            for (var rank = 0; rank < 13; rank++)
            {
                var card = new Card((Rank) rank, (Suit) suit);
                var expected = rank == 0 
                    ? 1UL << 13 << (suit * 16) 
                    : 1UL << rank << (suit * 16);
                card.AceHighBitMask.Should().Be(expected);
            }
        }
    }

    [Test]
    public void FromBitMask() => Card.FullDeck.Select(c => Card.FromBitMask(c.BitMask)).Should().BeEquivalentTo(Card.FullDeck);

    [Test]
    public void BitMaskToBitIndex_Fallback()
    {
        var actual = Card.FullDeck.Select(c => Card.BitMaskToBitIndex_Fallback(c.BitMask));
        var expected = Card.FullDeck.Select(c => c.BitIndex);
        actual.Should().BeEquivalentTo(expected, c => c.WithStrictOrdering());
    }

    [Test]
    public void BitMaskToBitIndex_Intrinsic()
    {
        if (!Bmi2.X64.IsSupported)
        {
            Assert.Inconclusive("Test cannot run as BMI2 intrinsics are not supported.");
        }
        var actual = Card.FullDeck.Select(c => Card.BitMaskToBitIndex_Intrinsic(c.BitMask));
        var expected = Card.FullDeck.Select(c => c.BitIndex);
        actual.Should().BeEquivalentTo(expected, c => c.WithStrictOrdering());
    }

    [Test]
    public void ToString_Test() => new Card(Rank.Ten, Suit.Clubs).ToString().Should().Be("10C");

    [TestCaseSource(nameof(EqualityTestCases))]
    public void Equality(Card x, Card y, bool expectedEqual) => EqualityTests.AssertEqual(x, y, expectedEqual);

    [Pure]
    public static IEnumerable<TestCaseData> EqualityTestCases()
    {
        var fullDeck = Card.FullDeck.ToList();
        for (var f = 0; f < 52; f++)
        {
            for (var g = 0; g < 52; g++)
            {
                yield return new TestCaseData(fullDeck[f], fullDeck[g], f == g);
            }
        }
    }
}