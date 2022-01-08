using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Collections;

public static class CombinationsExtensions
{
    [Pure]
    public static IEnumerable<IReadOnlyCardSet> Combinations([JetBrains.Annotations.InstantHandle] this IEnumerable<Card> source, int combinationSize)
    {
        if (source is IReadOnlyCollection<Card> collection)
        {
            return Combinations(collection, combinationSize);
        }
        
        var bitIndices = source.Select(c => c.BitIndex).ToArray();
        
        ValidateArguments(bitIndices, combinationSize);
        
        return Combinations(bitIndices, combinationSize);
    }

    [Pure]
    public static IEnumerable<IReadOnlyCardSet> Combinations(this IReadOnlyCollection<Card> source, int combinationSize)
    {
        if (source is IReadOnlyCardSet cardSet)
        {
            return Combinations(cardSet, combinationSize);
        }
        
        ValidateArguments(source, combinationSize);
        
        var bitIndices = new ulong[source.Count];
        var index = 0;
        foreach (var card in source)
        {
            bitIndices[index++] = card.BitIndex;
        }

        return Combinations(bitIndices, combinationSize);
    }
    
    [Pure]
    public static IEnumerable<IReadOnlyCardSet> Combinations(this IReadOnlyCardSet source, int combinationSize)
    {
        ValidateArguments(source, combinationSize);
        
        var bitIndices = new ulong[source.Count];
        var index = 0;
        foreach (var bitIndex in source.EnumerateBitIndices())
        {
            bitIndices[index++] = bitIndex;
        }

        return Combinations(bitIndices, combinationSize);
    }

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Combinations(IReadOnlyList<ulong> source, int combinationSize)
    {
        // Two stacks, one for the next index to process and one for the previous combination, i.e. the combination before that index was added.
        var indexStack = new int[combinationSize];
        var previousStack = new ulong[combinationSize];
        var stackCount = 1;

        while (stackCount > 0)
        {
            // Pop the stacks; get the next index to process and the previous combination.
            stackCount--;
            var index = indexStack[stackCount];
            var combination = previousStack[stackCount];

            // Loop through the next items in source, adding them to the combination.
            while (index < source.Count)
            {
                var toAdd = source[index++];

                // Push the next index and the combination before adding it onto the stacks.
                indexStack[stackCount] = index;
                previousStack[stackCount] = combination;
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

    private static void ValidateArguments<T>(this IReadOnlyCollection<T> source, int combinationSize)
    {
        if (source.Count == 0)
        {
            throw new ArgumentException("Value is empty.", nameof(source));
        }

        if (combinationSize <= 0 || combinationSize > source.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(combinationSize), combinationSize, $"Value must be greater than 0 and equal to or less than the size of {nameof(source)}. ({source.Count}");
        }
    }
}