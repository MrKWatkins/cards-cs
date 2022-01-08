using FluentAssertions;
using MrKWatkins.Cards.Collections;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Collections;

public sealed class CombinationsExtensionsTests
{
    [Test]
    public void Combinations_IEnumerable_EmptySourceThrows() => 
        FluentActions.Invoking(() => Enumerable.Empty<Card>().Combinations(5).ToList()).Should().Throw<ArgumentException>();
    
    [Test]
    public void Combinations_IReadOnlyCollection_EmptySourceThrows() => 
        FluentActions.Invoking(() => Array.Empty<Card>().Combinations(5).ToList()).Should().Throw<ArgumentException>();
    
    [Test]
    public void Combinations_IReadOnlyCardSet_EmptySourceThrows() => 
        FluentActions.Invoking(() => ImmutableCardSet.Empty.Combinations(5).ToList()).Should().Throw<ArgumentException>();
    
    [TestCase(-1)]
    [TestCase(-0)]
    [TestCase(53)]
    public void Combinations_IEnumerable_InvalidSizeThrows(int size) => 
        FluentActions.Invoking(() => Enumerable.Range(0, 52).Select(Card.FromIndex).Combinations(size).ToList()).Should().Throw<ArgumentOutOfRangeException>();
    
    [TestCase(-1)]
    [TestCase(-0)]
    [TestCase(53)]
    public void Combinations_IReadOnlyCollection_InvalidSizeThrows(int size) => 
        FluentActions.Invoking(() => Card.FullDeck().ToArray().Combinations(size).ToList()).Should().Throw<ArgumentOutOfRangeException>();
    
    [TestCase(-1)]
    [TestCase(-0)]
    [TestCase(53)]
    public void Combinations_IReadOnlyCardSet_InvalidSizeThrows(int size) => 
        FluentActions.Invoking(() => ImmutableCardSet.FullDeck.Combinations(size).ToList()).Should().Throw<ArgumentOutOfRangeException>();

    [Test]
    public void Combinations_IEnumerable() => AssertCombinations(Card.FullDeck().Take(7).Combinations(5), 5, 21);

    [Test]
    public void Combinations_IEnumerable_ShortCircuitForIReadOnlyCollection()
    {
        IEnumerable<Card> cards = Card.FullDeck().Take(7).ToArray();
        AssertCombinations(cards.Combinations(5), 5, 21);
    }
    
    [Test]
    public void Combinations_IEnumerable_ShortCircuitForIReadOnlyCardSet()
    {
        IEnumerable<Card> cards = new ImmutableCardSet(Card.FullDeck().Take(7));
        AssertCombinations(cards.Combinations(5), 5, 21);
    }
    
    [Test]
    public void Combinations_IReadOnlyCollection_ShortCircuitForIReadOnlyCardSet()
    {
        IReadOnlyCollection<Card> cards = new ImmutableCardSet(Card.FullDeck().Take(7));
        AssertCombinations(cards.Combinations(5), 5, 21);
    }

    [Test]
    public void Combinations_IReadOnlyCollection() => AssertCombinations(Card.FullDeck().Take(7).ToArray().Combinations(5), 5, 21);

    [Test]
    public void Combinations_IReadOnlyCardSet() => AssertCombinations(new CardSet(Card.FullDeck().Take(7)).Combinations(5), 5, 21);

    [Test]
    public void Combinations_FullDeck() => AssertCombinations(ImmutableCardSet.FullDeck.Combinations(5), 5, 2598960);

    private static void AssertCombinations([JetBrains.Annotations.InstantHandle] IEnumerable<IReadOnlyCardSet> combinations, int expectedSize, int expectedCount)
    {
        var combinationsList = combinations.ToList();
        combinationsList.Should().HaveCount(expectedCount);
        combinationsList.Should().OnlyContain(c => c.Count == expectedSize);

        // Not using combinations.Should().OnlyHaveUniqueItems() for the uniqueness test as it is much slower for large numbers of combinations.
        var unique = combinationsList.ToHashSet();
        unique.Should().HaveCount(expectedCount, "Card sets should be unique.");
    }
}