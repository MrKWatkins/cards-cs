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