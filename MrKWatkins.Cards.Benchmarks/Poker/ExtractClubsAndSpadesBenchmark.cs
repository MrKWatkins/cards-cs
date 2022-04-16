using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Benchmarks.Poker;

public class ExtractClubsAndSpadesBenchmark
{
    private static readonly IReadOnlyList<ulong> AllFiveCardHands = Card.FullDeck.Combinations(5)
        .Select(h => h.Aggregate(0UL, (current, card) => current | card.AceHighBitMask))
        .ToList();
    
    [Benchmark(Baseline = true)]
    public ulong[] NoIntrinsics() => RunTest(NoIntrinsics);
    
    [Benchmark]
    public ulong[] Intrinsics() => RunTest(Intrinsics);

    [Pure]
    private static ulong[] RunTest(Func<ulong, ulong> function)
    {
        var result = new ulong[52];

        for (var f = 0; f < 20; f++)
        {
            foreach (var hand in AllFiveCardHands)
            {
                function(hand);
            }
        }

        return result;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong NoIntrinsics(ulong value) => 0x00000000FFFFFFFF & ((value << 16) | (value >> 48));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ulong Intrinsics(ulong value) => Bmi2.X64.ParallelBitExtract(value, 0xFFFF00000000FFFF);
}