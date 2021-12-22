using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MrKWatkins.Cards.Collections;

internal static class BitIndexOperations
{
    internal const ulong FullDeckBitIndices = 0b00000000_00001111_11111111_11111111_11111111_11111111_11111111_11111111UL;
    
    [JetBrains.Annotations.MustUseReturnValue]
    internal static ulong ToBitIndices([JetBrains.Annotations.InstantHandle] IEnumerable<Card> cards)
    {
        if (cards is IReadOnlyCardSet cardSet)
        {
            return cardSet.BitIndices;
        }

        return cards.Aggregate(0UL, (current, card) => current | card.BitIndex);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static bool Contains(ulong x, ulong y) => (x & y) != 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong Except(ulong bitIndices, ulong toRemove) => bitIndices & ~toRemove;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong Intersect(ulong bitIndices, ulong toRemove) => bitIndices & toRemove;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static bool IsProperSubsetOf(ulong xBitIndices, ulong yBitIndices) => xBitIndices != yBitIndices && IsSubsetOf(xBitIndices, yBitIndices);
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static bool IsSubsetOf(ulong xBitIndices, ulong yBitIndices) => (xBitIndices & yBitIndices) == xBitIndices;
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static bool IsProperSupersetOf(ulong xBitIndices, ulong yBitIndices) => IsProperSubsetOf(yBitIndices, xBitIndices);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static bool IsSupersetOf(ulong xBitIndices, ulong yBitIndices) => IsSubsetOf(yBitIndices, xBitIndices);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static bool Overlaps(ulong xBitIndices, ulong yBitIndices) => (xBitIndices & yBitIndices) != 0UL;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong SymmetricExcept(ulong xBitIndices, ulong yBitIndices) => xBitIndices ^ yBitIndices;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong Union(ulong xBitIndices, ulong yBitIndices) => xBitIndices | yBitIndices;
}