using FluentAssertions;
using MrKWatkins.Cards.Text;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local

namespace MrKWatkins.Cards.Tests.Text;

public sealed class EnumFormatTests
{
    [Test]
    public void Constructor_ThrowsIfUnderlyingTypeNotInt() =>
        FluentActions.Invoking(() => new EnumFormat<ByteEnum>(true, "Zero", "One"))
            .Should().Throw<ArgumentException>()
            .And.Message.Should().StartWith("Enum type must have an underlying type of Int32.");

    [Test]
    public void Constructor_ThrowsIfUnderlyingTypeHasNoZeroValue() =>
        FluentActions.Invoking(() => new EnumFormat<NoZeroValue>(true, "One"))
            .Should().Throw<ArgumentException>()
            .And.Message.Should().StartWith("Enum type must have sequential values starting at 0.");

    [Test]
    public void Constructor_ThrowsIfUnderlyingTypeHasNonSequentialValues() =>
        FluentActions.Invoking(() => new EnumFormat<NonSequentialValues>(true, "Zero", "Two"))
            .Should().Throw<ArgumentException>()
            .And.Message.Should().StartWith("Enum type must have sequential values starting at 0.");

    [Test]
    public void Constructor_ThrowsIfWrongNumberOfValues() =>
        FluentActions.Invoking(() => new EnumFormat<Rank>(true, "One", "Two"))
            .Should().Throw<ArgumentException>()
            .And.Message.Should().StartWith("Value must have 13 entries.");

    private enum ByteEnum : byte
    {
        Zero, 
        One
    }
    
    private enum NoZeroValue
    {
        One = 1
    }
    
    private enum NonSequentialValues
    {
        Zero = 0,
        Two = 2
    }
}