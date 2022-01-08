using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class BitIndexExtensionsTests
{
    [Test]
    public void EnumerateBitIndices()
    {
        var cards = new[] { new Card(Rank.Ten, Suit.Spades), new Card(Rank.Four, Suit.Hearts) };
        var cardSet = new ImmutableCardSet(cards);
        cardSet.EnumerateBitIndices().Should().BeEquivalentTo(cards.Select(c => c.BitIndex), c => c.WithStrictOrdering());
    }
}