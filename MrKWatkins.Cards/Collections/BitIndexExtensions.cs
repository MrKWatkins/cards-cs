using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Collections;

public static class BitIndexExtensions
{
    [Pure]
    public static IEnumerable<ulong> EnumerateBitIndices(this IReadOnlyCardSet value) => value.BitIndices.EnumerateSetBits();
}