using System.Diagnostics.Contracts;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks;

[MemoryDiagnoser]
public class FullDeckBenchmark
{
    [Benchmark(Baseline = true)]
    public IReadOnlyList<Card> NestedForLoop() => Enumerate_NestedForLoop().ToList();

    [Benchmark]
    public IReadOnlyList<Card> NestedForLoopPreAllocate()
    {
        var list = new List<Card>(52);
        list.AddRange(Enumerate_NestedForLoop());
        return list;
    }
    
    [Benchmark]
    public IReadOnlyList<Card> Linq() => Enumerate_Linq().ToList();

    [Benchmark]
    public IReadOnlyList<Card> LinqPreAllocate()
    {
        var list = new List<Card>(52);
        list.AddRange(Enumerate_Linq());
        return list;
    }

    [Benchmark]
    public IReadOnlyList<Card> CardSet() => Collections.CardSet.CreateFullDeck().ToList();


    [Benchmark]
    public IReadOnlyList<Card> CardSetPreAllocate()
    {
        var list = new List<Card>(52);
        list.AddRange(Collections.CardSet.CreateFullDeck());
        return list;
    }

    [Pure]
    private static IEnumerable<Card> Enumerate_NestedForLoop()
    {
        foreach (var suit in Card.Suits)
        {
            foreach (var rank in Card.Ranks)
            {
                yield return new Card(rank, suit);
            }
        }
    }
    
    [Pure]
    private static IEnumerable<Card> Enumerate_Linq() => Card.Suits.SelectMany(_ => Card.Ranks, (suit, rank) => new Card(rank, suit));
}