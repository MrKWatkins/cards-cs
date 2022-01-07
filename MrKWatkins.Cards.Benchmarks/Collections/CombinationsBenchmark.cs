using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Benchmarks.Collections;

[MemoryDiagnoser]
public class CombinationsBenchmark
{
    private readonly IReadOnlyList<Card> fullDeck = Card.FullDeck().ToList();

    [Benchmark(Baseline = true)]
    public void Recursive()
    {
        foreach (var _ in Recursive(fullDeck, 5, ImmutableCardSet.Empty, 0))
        {
        }
    }

    [Benchmark]
    public void Iterative()
    {
        foreach (var _ in fullDeck.Combinations(5))
        {
        }
    }

    [Pure]
    private static IEnumerable<IImmutableSet<Card>> Recursive(IReadOnlyList<Card> source, int combinationSize, IImmutableSet<Card> currentCombination, int startSourceIndex)
    {
        if (combinationSize == currentCombination.Count)
        {
            yield return currentCombination;
            yield break;
        }

        for (var f = startSourceIndex; f < source.Count; f++)
        {
            foreach (var result in Recursive(source, combinationSize, currentCombination.Add(source[f]), f + 1))
            {
                yield return result;
            }
        }
    }
}