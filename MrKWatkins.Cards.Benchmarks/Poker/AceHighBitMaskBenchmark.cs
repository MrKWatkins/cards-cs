using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks.Poker;

public class AceHighBitMaskBenchmark
{
    [Benchmark(Baseline = true)]
    public ulong[] Branching() => RunTest(Branching);
    
    [Benchmark]
    public ulong[] NoBranching() => RunTest(NoBranching);
    
    [Benchmark]
    public ulong[] NoBranchingOrMultiplication() => RunTest(NoBranchingOrMultiplication);
    
    [Pure]
    private static ulong[] RunTest(Func<Card, ulong> function)
    {
        var result = new ulong[52];
        for (var f = 0; f < 52; f++)
        {
            result[f] = function(Card.FullDeck[f]);
        }
        return result;
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong Branching(Card card) => 
        card.Rank == Rank.Ace 
            ? 1UL << 13 << ((int) card.Suit * 16) 
            : 1UL << (int) card.Rank << ((int) card.Suit * 16);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong NoBranching(Card card)
    {
        var rank = 1UL << (int)card.Rank;
        var aceHigh = rank | (rank << 13);
        var cleared = aceHigh & 0b11111111111110;
        
        return cleared << ((int)card.Suit * 16);
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong NoBranchingOrMultiplication(Card card)
    {
        var rank = 1UL << (int)card.Rank;
        var aceHigh = rank | (rank << 13);
        var cleared = aceHigh & 0b11111111111110;
        
        return cleared << ((int)card.Suit << 4);
    }
}