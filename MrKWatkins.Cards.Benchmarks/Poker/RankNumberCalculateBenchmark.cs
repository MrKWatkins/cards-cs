using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Benchmarks.Poker;

public class RankNumberCalculateBenchmark
{
    private static readonly IReadOnlyList<IReadOnlyList<Card>> AllFiveCardHands = Card.FullDeck.Combinations(5).Select(h => h.ToList()).ToList();
    private static readonly Vector128<short> Multiply = Vector128.Create(
        Vector64.Create(13 * 13 * 13 * 13, 13 * 13 * 13, 13 * 13, 13),
        Vector64.Create(1, 0, 0, 0));

    // ReSharper disable once NotAccessedField.Local
    private static int result;
    
    [Benchmark(Baseline = true)]
    public void NonIntrinsic()
    {
        for (var f = 0; f < 5; f++)
        {
            foreach (var hand in AllFiveCardHands)
            {
                result = (int)hand[0].Rank +
                         (int)hand[1].Rank * 13 +
                         (int)hand[2].Rank * 13 * 13 +
                         (int)hand[3].Rank * 13 * 13 * 13 +
                         (int)hand[4].Rank * 13 * 13 * 13 * 13;
            }
        }
    }
    
    [Benchmark]
    public void Intrinsic()
    {
        for (var f = 0; f < 5; f++)
        {
            foreach (var hand in AllFiveCardHands)
            {
                var vector = Vector128.Create((short)hand[0].Rank, (short)hand[1].Rank, (short)hand[2].Rank, (short)hand[3].Rank, (short)hand[4].Rank, 0, 0, 0);

                var number = Sse2.MultiplyAddAdjacent(vector, Multiply);

                number = Ssse3.HorizontalAdd(number, number);
                number = Ssse3.HorizontalAdd(number, number);

                result = number.ToScalar();
            }
        }
    }
    
    [Benchmark]
    public void Intrinsic2()
    {
        for (var f = 0; f < 5; f++)
        {
            foreach (var hand in AllFiveCardHands)
            {
                var vector = Vector128.Create((short)hand[0].Rank, (short)hand[1].Rank, (short)hand[2].Rank, (short)hand[3].Rank, 0, 0, 0, 0);

                var number = Sse2.MultiplyAddAdjacent(vector, Multiply);

                number = Ssse3.HorizontalAdd(number, number);

                result = number.ToScalar() + (int)hand[4].Rank;
            }
        }
    }
}