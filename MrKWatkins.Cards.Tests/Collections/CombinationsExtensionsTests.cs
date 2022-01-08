using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class CombinationsExtensionsTests
{
    [Test]
    public void Combinations_EmptySourceThrows() => FluentActions.Invoking(() => Array.Empty<Card>().Combinations(5).ToList()).Should().Throw<ArgumentException>();
    
    [TestCase(-1)]
    [TestCase(-0)]
    [TestCase(53)]
    public void Combinations_InvalidSizeThrows(int size) => FluentActions.Invoking(() => Card.FullDeck().Combinations(size).ToList()).Should().Throw<ArgumentOutOfRangeException>();
    
    [Test]
    public void Combinations()
    {
        var combinations = Card.FullDeck().Combinations(5).ToList();
        combinations.Should().HaveCount(2598960);
        combinations.Should().OnlyContain(c => c.Count == 5);

        var unique = combinations.ToHashSet();
        unique.Should().HaveCount(2598960);
    }
    
    [Test]
    public void Combinations2()
    {
        var combinations = Card.FullDeck().Combinations2(5).ToList();
        combinations.Should().HaveCount(2598960);
        combinations.Should().OnlyContain(c => c.Count == 5);

        var unique = combinations.ToHashSet();
        unique.Should().HaveCount(2598960);
    }
}