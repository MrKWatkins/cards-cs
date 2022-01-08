using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace MrKWatkins.Cards;

internal static class BitOperationExtensions
{
    private const ulong Bits0To15 = 0x000000000000FFFF;
    
    /// <summary>
    /// Negates a <see cref="ulong"/> value, i.e. treats it as if it were a <see cref="long"/> and performs the - operation.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong Negate(this ulong value) => ~value + 1;
    
    /// <summary>
    /// Resets the rightmost bit in a value.
    /// </summary>
    /// <remarks>
    /// Hacker's Delight, second edition, page 11.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong ResetLowestSetBit(this ulong value) => 
        Bmi1.X64.IsSupported ? Bmi1.X64.ResetLowestSetBit(value) : value & (value - 1);

    /// <summary>
    /// Returns a value with a single bit set, corresponding to the rightmost bit of <see cref="value"/>, or 0 if value is 0.
    /// </summary>
    /// <remarks>
    /// Hacker's Delight, second edition, page 12.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong ExtractLowestSetBit(this ulong value) => 
        Bmi1.X64.IsSupported ? Bmi1.X64.ExtractLowestSetBit(value) : value & Negate(value);

    /// <summary>
    /// Returns the number of set bits in <paramref name="value"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PopCount(this ulong value) => BitOperations.PopCount(value);
    
    /// <summary>
    /// Returns the number of trailing zeros <paramref name="value"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TrailingZeroCount(this ulong value) => BitOperations.TrailingZeroCount(value);
    
    /// <summary>
    /// Treats the 64 bit <see cref="ulong"/> as 4 x 16 bit <see cref="ushort"/>s. ORs the 4 x 16 bits together, returning
    /// the value in the lowest 16 bits of a ulong with the other bits all set to 0.
    /// </summary>
    /// <remarks>
    /// The resultant 16 bits will contain a set bit for any bit that was set in any of the original 4 x 16 bits.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong HorizontalOr16(this ulong value)
    {
        // OR bits 0-31 with 32-63, i.e. one half with the other.
        value |= value >> 32;
        // Then bits 0-15 with 16-31, i.e. one half with the other of the 32 bits.
        value |= value >> 16;

        // Clear out any junk.
        return value & Bits0To15;
    }
    
    /// <summary>
    /// Treats the 64 bit <see cref="ulong"/> as 4 x 16 bit <see cref="ushort"/>s. ANDs the 4 x 16 bits together, returning
    /// the value in the lowest 16 bits of a ulong with the other bits all set to 0.
    /// </summary>
    /// <remarks>
    /// The resultant 16 bits will contain a set bit for any bit that was set in all of the original 4 x 16 bits.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong HorizontalAnd16(this ulong value)
    {
        value &= value >> 32;

        // No need to clear any junk as we've been ANDing against 0s in the higher bits meaning they'll already be 0.
        return value & (value >> 16);
    }
    
    /// <summary>
    /// Treats the 64 bit <see cref="ulong"/> as 4 x 16 bit <see cref="ushort"/>s. XORs the 4 x 16 bits together, returning
    /// the value in the lowest 16 bits of a ulong with the other bits all set to 0.
    /// </summary>
    /// <remarks>
    /// The resultant 16 bits will contain a set bit for any bit that was set in an odd number of the original 4 x 16 bits.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong HorizontalXor16(this ulong value)
    {
        value ^= value >> 32;
        value ^= value >> 16;

        return value & Bits0To15;
    }
}