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
    
    [TestCase(0UL, 0)]
    [TestCase(0b0000000000010000_0000000000001000_0000000000000000_0100000000001000UL, 4)]
    public void PopCount(ulong value, int expected) => value.PopCount().Should().Be(expected);
    
    [TestCase(0UL, 64)]
    [TestCase(0b1000000000010000_0000000000001000_0000000000000000_0110000000100001UL, 0)]
    [TestCase(0b1000000000010000_0000000000001000_0000000000000000_0110000000100000UL, 5)]
    public void TrailingZeroCount(ulong value, int expected) => value.TrailingZeroCount().Should().Be(expected);
    
    [TestCase(0UL, 0UL)]
    [TestCase(0b0000000000010000_0000000000001000_0000000000000000_0100000000001000UL, 0b0000000000000000_0000000000000000_0000000000000000_0100000000011000UL)]
    public void HorizontalOr16(ulong value, ulong expected) => value.HorizontalOr16().Should().Be(expected);
    
    [TestCase(0UL, 0UL)]
    [TestCase(0b0000000000011000_0000000000001000_0110000000001000_0100000000001000UL, 0b0000000000000000_0000000000000000_0000000000000000_0000000000001000UL)]
    public void HorizontalAnd16(ulong value, ulong expected) => value.HorizontalAnd16().Should().Be(expected);
    
    [TestCase(0UL, 0UL)]
    [TestCase(0b1000000000011000_0000000000011000_0000000000011000_0000000000001000UL, 0b0000000000000000_0000000000000000_0000000000000000_1000000000010000UL)]
    public void HorizontalXor16(ulong value, ulong expected) => value.HorizontalXor16().Should().Be(expected);
}