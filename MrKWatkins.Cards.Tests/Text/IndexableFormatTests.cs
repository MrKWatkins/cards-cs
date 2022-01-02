using FluentAssertions;
using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Text;

public sealed class IndexableFormatTests
{
    [Test]
    public void Constructor_ThrowsIfValuesContainsSeparator() =>
        FluentActions.Invoking(() => new TestIndexableFormat("T", false))
            .Should().Throw<ArgumentException>()
            .And.Message.Should().StartWith("Value \"T\" is contained in one or more of values.");

    [Test]
    public void Constructor_ThrowsForCaseInsensitiveParsingAndNonInvariantSeparator()
    {
        FluentActions.Invoking(() => new TestIndexableFormat(",", true)).Should().NotThrow();
        FluentActions.Invoking(() => new TestIndexableFormat("X", false)).Should().NotThrow();
        
        FluentActions.Invoking(() => new TestIndexableFormat("X", true))
            .Should().Throw<ArgumentException>()
            .And.Message.Should().StartWith("Value \"X\" should be invariant with respect to case because caseInsensitiveParsing is true.");
    }

    private sealed class TestIndexableFormat : IndexableFormat<int>
    {
        public TestIndexableFormat(string multipleSeparator, bool caseInsensitiveParsing) 
            : base(multipleSeparator, caseInsensitiveParsing, new [] { "One", "Two", "Three" })
        {
        }

        protected override int ToIndex(int value) => value;

        protected override int ToValue(int index) => index;
    }
}