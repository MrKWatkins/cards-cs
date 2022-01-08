using System.Diagnostics.Contracts;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks;

[MemoryDiagnoser]
public class CardFromBitMaskBenchmark
{
    private static readonly IReadOnlyList<ulong> BitMasks = Card.FullDeck.Select(c => c.BitMask).ToArray();

    [Benchmark(Baseline = true)]
    public Card[] ConvertToBitIndex() => RunTest(ConvertToBitIndex);
    
    [Benchmark]
    public Card[] ConvertToBitIndexNoBranching() => RunTest(ConvertToBitIndexNoBranching);
    
    [Benchmark]
    public Card[] ConvertToBitIndexIntrinsic() => RunTest(ConvertToBitIndexIntrinsic);

    [Benchmark]
    public Card[] IsolateSuitThenTrailingZeroCount() => RunTest(IsolateSuitThenTrailingZeroCount);

    [Benchmark]
    public Card[] TrailingZeroCountThenIsolateSuit() => RunTest(TrailingZeroCountThenIsolateSuit);

    [Pure]
    private static Card[] RunTest(Func<ulong, Card> function)
    {
        var result = new Card[52];
        for (var f = 0; f < 52; f++)
        {
            result[f] = function(BitMasks[f]);
        }

        return result;
    }

    [Pure]
    private static Card ConvertToBitIndex(ulong bitMask)
    {
        // Bit indices: Spades = 0 -> 12, Hearts = 13 -> 25, Diamonds = 26 -> 38, Clubs = 39 -> 51.
        // Bit masks: Spades = 0 -> 12, Hearts = 16 -> 28, Diamonds = 32 -> 44, Clubs = 48 -> 60.
        
        // Is it spades or heart?
        if ((bitMask & 0xFFFFFFFF00000000) == 0)
        {
            if ((bitMask & 0x00000000FFFF0000) == 0)
            {
                // Spades; move into the correct position.
                return Card.FromBitIndex(bitMask);
            }
            
            // Hearts; move into the correct position.
            return Card.FromBitIndex(bitMask >> 3);
        }

        if ((bitMask & 0x0000FFFF00000000) == 0)
        {
            // Clubs; move into the correct position.
            return Card.FromBitIndex(bitMask >> 9);
        }
            
        // Diamonds; move into the correct position.
        return Card.FromBitIndex(bitMask >> 6);
    }
    
    [Pure]
    private static Card ConvertToBitIndexNoBranching(ulong bitMask)
    {
        // Bit indices: Spades = 0 -> 12, Hearts = 13 -> 25, Diamonds = 26 -> 38, Clubs = 39 -> 51.
        // Bit masks: Spades = 0 -> 12, Hearts = 16 -> 28, Diamonds = 32 -> 44, Clubs = 48 -> 60.
        var spades = bitMask & 0x000000000000FFFF;
        var hearts = (bitMask & 0x00000000FFFF0000) >> 3;
        var diamonds = (bitMask & 0x0000FFFF00000000) >> 6;
        var clubs = (bitMask & 0xFFFF000000000000) >> 9;
        return Card.FromBitIndex(spades | hearts | diamonds | clubs);
    }
    
    [Pure]
    private static Card IsolateSuitThenTrailingZeroCount(ulong bitMask)
    {
        // Is it spades or hearts, i.e. in bits 0 -> 31?
        if ((bitMask & 0xFFFFFFFF00000000) == 0)
        {
            // Yes. Is it hearts, i.e. in bits 16 -> 31?
            if ((bitMask & 0x00000000FFFF0000) == 0)
            {
                return new Card((Rank) bitMask.TrailingZeroCount(), Suit.Spades);
            }
            return new Card((Rank)(bitMask.TrailingZeroCount() - 16), Suit.Hearts);
        }
        
        // Yes. Is it clubs, i.e. in bits 48 -> 64?
        if ((bitMask & 0xFFFF000000000000) == 0)
        {
            return new Card((Rank)(bitMask.TrailingZeroCount() - 32), Suit.Diamonds);
        }
        return new Card((Rank)(bitMask.TrailingZeroCount() - 48), Suit.Clubs);
    }
    
    [Pure]
    private static Card TrailingZeroCountThenIsolateSuit(ulong bitMask)
    {
        var trailingZeros = bitMask.TrailingZeroCount();
        if (trailingZeros >= 32)
        {
            if (trailingZeros >= 48)
            {
                return new Card((Rank)(trailingZeros - 48), Suit.Clubs);
            }
            return new Card((Rank)(trailingZeros - 32), Suit.Diamonds);
        }
        if (trailingZeros >= 16)
        {
            return new Card((Rank)(trailingZeros - 16), Suit.Hearts);
        }
        return new Card((Rank)trailingZeros, Suit.Spades);
    }
    
    [Pure]
    private static Card ConvertToBitIndexIntrinsic(ulong bitMask)
    {
        if (Bmi2.X64.IsSupported)
        {
            // https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#text=_pext_u64
            // Bit masks: Spades = 0 -> 12, Hearts = 16 -> 28, Diamonds = 32 -> 44, Clubs = 48 -> 60.
            // Bit indices: Spades = 0 -> 12, Hearts = 13 -> 25, Diamonds = 26 -> 38, Clubs = 39 -> 51.
            // _pext_u64 extracts bits according to a mask and places them in the contiguous low bits.
            const ulong mask = 0b0001111111111111_0001111111111111_0001111111111111_0001111111111111;
            var bitIndex = Bmi2.X64.ParallelBitExtract(bitMask, mask);
            return Card.FromBitIndex(bitIndex);
        }

        throw new NotSupportedException("_pext_u64 is not supported.");
    }
}