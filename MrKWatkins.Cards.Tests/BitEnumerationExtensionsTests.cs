using FluentAssertions;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests;

public sealed class BitEnumerationExtensionsTests
{
    [Test]
    public void EnumerateSetBits_Empty() => 0UL.EnumerateSetBits().Should().BeEmpty();

    [Test]
    public void EnumerateSetBits() => 0b0001_1000UL.EnumerateSetBits().Should().BeEquivalentTo(new[] { 0b0000_1000UL, 0b0001_0000UL }, c => c.WithStrictOrdering());

    [Test]
    public void EnumerateSetBits_Enumerator_Reset()
    {
        using var enumerator = 0b1001_1000UL.EnumerateSetBits().GetEnumerator();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be(0b0000_1000UL);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be(0b0001_0000UL);
        
        enumerator.Reset();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be(0b0000_1000UL);
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be(0b0001_0000UL);
    }
}