using FluentAssertions;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests;

public sealed class BitOperationExtensionsTests
{
    [TestCase(0L, 0L)]
    [TestCase(1L, -1L)]
    [TestCase(1000L, -1000L)]
    [TestCase(-1L, 1L)]
    [TestCase(-1000L, 1000L)]
    public void Negate(long value, long expected)
    {
        var ulongValue = (ulong)value;
        var ulongExpected = (ulong)expected;
        ulongValue.Negate().Should().Be(ulongExpected);
    }
    
    [TestCase(0UL, 0UL)]
    [TestCase(0b1111UL, 0b1110UL)]
    [TestCase(0b1010UL, 0b1000UL)]
    [TestCase(0b1110UL, 0b1100UL)]
    [TestCase(0b1100UL, 0b1000UL)]
    [TestCase(0b1000UL, 0b0000UL)]
    public void ResetRightmostBit(ulong value, ulong expected) => value.ResetLowestSetBit().Should().Be(expected);
    
    [TestCase(0UL, 0UL)]
    [TestCase(0b1111UL, 0b0001UL)]
    [TestCase(0b1010UL, 0b0010UL)]
    [TestCase(0b1110UL, 0b0010UL)]
    [TestCase(0b1100UL, 0b0100UL)]
    [TestCase(0b1000UL, 0b1000UL)]
    public void ExtractRightmostBit(ulong value, ulong expected) => value.ExtractLowestSetBit().Should().Be(expected);
}