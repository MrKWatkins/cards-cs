using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Benchmarks.Collections;

[MemoryDiagnoser]
public class CombinationsBenchmark
{
    private readonly IReadOnlyList<Card> fullDeck = Card.FullDeck.ToList();

    [Benchmark(Baseline = true)]
    public void Recursive()
    {
        foreach (var _ in Recursive(fullDeck, 5, ImmutableCardSet.Empty, 0))
        {
        }
    }

    [Benchmark]
    public void Iterative_Stack()
    {
        foreach (var _ in Iterative_Stack(fullDeck, 5))
        {
        }
    }
    
    [Benchmark]
    public void Iterative_ArrayForStack()
    {
        foreach (var _ in Iterative_ArrayForStack(fullDeck, 5))
        {
        }
    }
    
    [Benchmark]
    public void Iterative_ArrayForStackAndPrevious()
    {
        foreach (var _ in Iterative_ArrayForStackAndPrevious(fullDeck, 5))
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

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_Stack(IReadOnlyList<Card> source, int combinationSize)
        => Iterative_Stack(source.Select(c => c.BitIndex).ToList(), combinationSize);

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_Stack(IReadOnlyList<ulong> source, int combinationSize)
    {
        var combination = 0UL;
        var stack = new Stack<int>(combinationSize);
        stack.Push(0);
 
        while (stack.Count > 0)
        {
            var toAddIndex = stack.Pop();
            if (toAddIndex > 0)
            {
                combination = BitIndexOperations.Except(combination, source[toAddIndex - 1]);
            }
            var size = stack.Count;

            while (toAddIndex < source.Count)
            {
                var toAdd = source[toAddIndex++];
                combination = BitIndexOperations.Union(combination, toAdd);
                stack.Push(toAddIndex);
                size++;
 
                if (size == combinationSize) 
                {
                    yield return new ImmutableCardSet(combination);
                    break;
                }
            }
        }
    }
    
    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_ArrayForStack(IReadOnlyList<Card> source, int combinationSize)
        => Iterative_ArrayForStack(source.Select(c => c.BitIndex).ToList(), combinationSize);

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_ArrayForStack(IReadOnlyList<ulong> source, int combinationSize)
    {
        var combination = 0UL;
        var indexStack = new int[combinationSize];
        var stackCount = 1;
 
        while (stackCount > 0)
        {
            stackCount--;
            var toAddIndex = indexStack[stackCount];
            if (toAddIndex > 0)
            {
                combination = BitIndexOperations.Except(combination, source[toAddIndex - 1]);
            }

            while (toAddIndex < source.Count)
            {
                var toAdd = source[toAddIndex++];
                combination = BitIndexOperations.Union(combination, toAdd);
                indexStack[stackCount] = toAddIndex;
                stackCount++;
 
                if (stackCount == combinationSize) 
                {
                    yield return new ImmutableCardSet(combination);
                    break;
                }
            }
        }
    }
    
    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_ArrayForStackAndPrevious(IReadOnlyList<Card> source, int combinationSize)
        => Iterative_ArrayForStackAndPrevious(source.Select(c => c.BitIndex).ToList(), combinationSize);

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_ArrayForStackAndPrevious(IReadOnlyList<ulong> source, int combinationSize)
    {
        var indexStack = new int[combinationSize];
        var previousStack = new ulong[combinationSize];
        var stackCount = 1;
 
        while (stackCount > 0)
        {
            stackCount--;
            var toAddIndex = indexStack[stackCount];
            var combination = previousStack[stackCount];

            while (toAddIndex < source.Count)
            {
                var toAdd = source[toAddIndex++];
                
                indexStack[stackCount] = toAddIndex;
                previousStack[stackCount] = combination;
                stackCount++;
                
                combination = BitIndexOperations.Union(combination, toAdd);
 
                if (stackCount == combinationSize) 
                {
                    yield return new ImmutableCardSet(combination);
                    break;
                }
            }
        }
    }
}