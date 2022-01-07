using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Collections;

public static class CombinationsExtensions
{
    [Pure]
    public static IEnumerable<IReadOnlyCardSet> Combinations([JetBrains.Annotations.InstantHandle] this IEnumerable<Card> source, int combinationSize)
        => source.Select(c => c.BitIndex).ToList().Combinations(combinationSize);

    [Pure]
    private static IEnumerable<IReadOnlyCardSet> Combinations(this IReadOnlyList<ulong> source, int combinationSize)
    {
        if (source.Count == 0)
        {
            throw new ArgumentException("Value is empty.", nameof(source));
        }

        if (combinationSize <= 0 || combinationSize > source.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(combinationSize), combinationSize, $"Value must be greater than 0 and equal to or less than the size of {nameof(source)}. ({source.Count}");
        }
        
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
}