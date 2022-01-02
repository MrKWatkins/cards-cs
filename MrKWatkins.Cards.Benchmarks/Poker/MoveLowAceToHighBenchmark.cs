using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks.Poker;

public class MoveLowAceToHighBenchmark
{
    private const ulong AceLowRankMask = 1UL;
    private const ulong AceHighRankMask = 1UL << 13;
    private const ulong AceHighAndLowRankMask = AceHighRankMask | AceLowRankMask;
    private static readonly IReadOnlyList<ulong> AllRankMasks = Card.Ranks.Select(r => 1UL << (int)r).ToArray();

    [Benchmark(Baseline = true)]
    public ulong[] Branching() => RunTest(Branching);
    
    [Benchmark]
    public ulong[] NoBranching() => RunTest(NoBranching);
    
    [Pure]
    private static ulong[] RunTest(Func<ulong, ulong> function)
    {
        var result = new ulong[13];
        for (var f = 0; f < 13; f++)
        {
            result[f] = function(AllRankMasks[f]);
        }

        return result;
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong Branching(ulong handMask)
    {
        if ((handMask & AceLowRankMask) == 0)
        {
            // No ace!
            return handMask;
        }

        // Using XOR will reset the low ace bit because it is currently 1 but set the high ace bit 
        // because it is currently 0.
        return handMask ^ AceHighAndLowRankMask;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong NoBranching(ulong handMask)
    {
        var ace = handMask & 1;
        var aceHigh = ace << 13;
        var withoutAce = handMask & 0b1111111111111110;

        return withoutAce | aceHigh;
    }
}