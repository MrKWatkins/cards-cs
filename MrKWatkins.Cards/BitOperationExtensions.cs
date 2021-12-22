using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace MrKWatkins.Cards;

internal static class BitOperationExtensions
{
    /// <summary>
    /// Negates a <see cref="ulong"/> value, i.e. treats it as if it were a <see cref="long"/> and performs the - operation.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong Negate(this ulong value) => ~value + 1;
    
    /// <summary>
    /// Resets the rightmost bit in a value.
    /// </summary>
    /// <remarks>
    /// Hacker's Delight, second edition, page 11.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong ResetLowestSetBit(this ulong value) => 
        Bmi1.X64.IsSupported ? Bmi1.X64.ResetLowestSetBit(value) : value & (value - 1);

    /// <summary>
    /// Returns a value with a single bit set, corresponding to the rightmost bit of <see cref="value"/>, or 0 if value is 0.
    /// </summary>
    /// <remarks>
    /// Hacker's Delight, second edition, page 12.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static ulong ExtractLowestSetBit(this ulong value) => 
        Bmi1.X64.IsSupported ? Bmi1.X64.ExtractLowestSetBit(value) : value & Negate(value);
}