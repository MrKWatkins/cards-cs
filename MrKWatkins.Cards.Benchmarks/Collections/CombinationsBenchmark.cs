using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Benchmarks.Collections;

public class CombinationsBenchmark
{
    private readonly IReadOnlyList<Card> fullDeck = Card.FullDeck.ToList();

    [Benchmark]
    public void Recursive()
    {
        foreach (var _ in Recursive(fullDeck, 5))
        {
        }
    }

    [Benchmark(Baseline = true)]
    public void Iterative()
    {
        foreach (var _ in Iterative(fullDeck, 5))
        {
        }
    }

    [Benchmark]
    public void Iterative_Array()
    {
        foreach (var _ in Iterative_Array(fullDeck, 5))
        {
        }
    }

    [Benchmark]
    public void Iterative_TwoArrays()
    {
        foreach (var _ in Iterative_TwoArrays(fullDeck, 5))
        {
        }
    }
    
    [Benchmark]
    public void Iterative_TwoArrays_BitIndices()
    {
        foreach (var _ in Iterative_TwoArrays_BitIndices(fullDeck, 5))
        {
        }
    }
    
    [Pure]
    private static IEnumerable<IImmutableSet<Card>> Recursive(IReadOnlyList<Card> source, int combinationSize) => Recursive(source, combinationSize, ImmutableCardSet.Empty, 0);
    
    [Pure]
    private static IEnumerable<IImmutableSet<Card>> Recursive(IReadOnlyList<Card> source, int combinationSize, IImmutableSet<Card> currentCombination, int startSourceIndex)
    {
        // Reached the desired size; return the combination.
        if (combinationSize == currentCombination.Count)
        {
            yield return currentCombination;
            yield break;
        }
        
        // Start from the current index in our source. We will add each card in turn from that index onwards to the combination.
        for (var f = startSourceIndex; f < source.Count; f++)
        {
            // Add the card and proceed recursively from the next index to fill up the combination.
            foreach (var result in Recursive(source, combinationSize, currentCombination.Add(source[f]), f + 1))
            {
                yield return result;
            }
        }
    }
    
    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative(IReadOnlyList<Card> source, int combinationSize)
    {
        // Stack of the current combination and the next index to start with for that combination.
        var stack = new Stack<(ImmutableCardSet CurrentCombination, int StartSourceIndex)>(combinationSize);
        stack.Push((ImmutableCardSet.Empty, 0));
 
        // Whilst the stack isn't empty we have more cards to process.
        while (stack.Count > 0)
        {
            // Pop the stack to get the next index to process.
            var (combination, startSourceIndex) = stack.Pop();

            // Loop over the remaining cards to add them to the combination.
            while (startSourceIndex < source.Count)
            {
                // Add the next card.
                var toAdd = source[startSourceIndex];
                startSourceIndex++;
                
                // Push the combination and the position of the next card to add onto the stack so we come back to them later.
                stack.Push((combination, startSourceIndex));
 
                combination = combination.Add(toAdd);
                
                // If we've reached the desired size return the combination.
                if (stack.Count == combinationSize) 
                {
                    yield return combination;
                    break;
                }
            }
        }
    }
    
    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_Array(IReadOnlyList<Card> source, int combinationSize)
    {
        // Array and position to use as a stack of the current combination and the next index to start with for that combination.
        var stack = new (ImmutableCardSet CurrentCombination, int StartSourceIndex)[combinationSize];
        var stackPosition = 1;
 
        // Whilst the stack isn't empty we have more cards to process.
        while (stackPosition > 0)
        {
            // Pop the stack to get the next index to process.
            stackPosition--;
            var (combination, startSourceIndex) = stack[stackPosition];

            // Loop over the remaining cards to add them to the combination.
            while (startSourceIndex < source.Count)
            {
                // Add the next card.
                var toAdd = source[startSourceIndex];
                startSourceIndex++;
                
                // Push the combination and the position of the next card to add onto the stack so we come back to them later.
                stack[stackPosition] = (combination, startSourceIndex);
                stackPosition++;
                
                combination = combination.Add(toAdd);
                
                // If we've reached the desired size return the combination.
                if (stackPosition == combinationSize) 
                {
                    yield return combination;
                    break;
                }
            }
        }
    }

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_TwoArrays(IReadOnlyList<Card> source, int combinationSize)
    {
        // Two arrays and position to use as a stack of the current combination and the next index to start with for that combination.
        var combinationStack = new ImmutableCardSet[combinationSize];
        var startSourceIndexStack = new int[combinationSize];
        var stackPosition = 1;
 
        // Whilst the stack isn't empty we have more cards to process.
        while (stackPosition > 0)
        {
            // Pop the stacks to get the combination and next index to process.
            stackPosition--;
            var combination = combinationStack[stackPosition];
            var startSourceIndex = startSourceIndexStack[stackPosition];

            // Loop over the remaining cards to add them to the combination.
            while (startSourceIndex < source.Count)
            {
                // Add the next card.
                var toAdd = source[startSourceIndex];
                startSourceIndex++;
                
                // Push the combination and the position of the next card to add onto the stack so we come back to them later.
                combinationStack[stackPosition] = combination;
                startSourceIndexStack[stackPosition] = startSourceIndex;
                stackPosition++;
                
                combination = combination.Add(toAdd);
 
                // If we've reached the desired size return the combination.
                if (stackPosition == combinationSize) 
                {
                    yield return combination;
                    break;
                }
            }
        }
    }
    
    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_TwoArrays_BitIndices(IReadOnlyList<Card> source, int combinationSize)
        => Iterative_TwoArrays_BitIndices(source.Select(c => c.BitIndex).ToList(), combinationSize);

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Iterative_TwoArrays_BitIndices(IReadOnlyList<ulong> source, int combinationSize)
    {
        // Two arrays and position to use as a stack of the current combination and the next index to start with for that combination.
        var combinationStack = new ulong[combinationSize];
        var startSourceIndexStack = new int[combinationSize];
        var stackCount = 1;

        // Whilst the stack isn't empty we have more cards to process.
        while (stackCount > 0)
        {
            // Pop the stacks to get the combination and next index to process.
            stackCount--;
            var combination = combinationStack[stackCount];
            var startSourceIndex = startSourceIndexStack[stackCount];

            // Loop over the remaining cards to add them to the combination.
            while (startSourceIndex < source.Count)
            {
                // Add the next card.
                var toAdd = source[startSourceIndex];
                startSourceIndex++;

                // Push the next index and the combination before adding it onto the stacks.
                startSourceIndexStack[stackCount] = startSourceIndex;
                combinationStack[stackCount] = combination;
                stackCount++;

                // Update the combination.
                combination = BitIndexOperations.Union(combination, toAdd);

                // If we've reached the desired size yield the combination.
                if (stackCount == combinationSize)
                {
                    yield return new ImmutableCardSet(combination);
                    break;
                }
            }
        }
    }
}