using System.Diagnostics.Contracts;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks;

[MemoryDiagnoser]
public class FullDeckBenchmark
{
    [Benchmark(Baseline = true)]
    public IReadOnlyList<Card> NestedForeachLoops() => Enumerate_NestedForeachLoops().ToList();

    [Benchmark]
    public IReadOnlyList<Card> NestedForeachLoopsPreAllocate()
    {
        var list = new List<Card>(52);
        list.AddRange(Enumerate_NestedForeachLoops());
        return list;
    }
    
    [Benchmark]
    public IReadOnlyList<Card> NestedForLoops() => Enumerate_NestedForLoops().ToList();

    [Benchmark]
    public IReadOnlyList<Card> NestedForLoopsPreAllocate()
    {
        var list = new List<Card>(52);
        list.AddRange(Enumerate_NestedForLoops());
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

    [Benchmark]
    public IReadOnlyList<Card> ImmutableCardSet() => Collections.ImmutableCardSet.FullDeck.ToList();

    [Benchmark]
    public IReadOnlyList<Card> ImmutableCardSetPreAllocate()
    {
        var list = new List<Card>(52);
        list.AddRange(Collections.ImmutableCardSet.FullDeck);
        return list;
    }

    [Pure]
    private static IEnumerable<Card> Enumerate_NestedForeachLoops()
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
    private static IEnumerable<Card> Enumerate_NestedForLoops()
    {
        for (var suit = 0; suit < 4; suit++)
        {
            for (var rank = 0; rank < 13; rank++)
            {
                yield return new Card((Rank) rank, (Suit) suit);
            }
        }
    }
    
    [Pure]
    private static IEnumerable<Card> Enumerate_Linq() => Card.Suits.SelectMany(_ => Card.Ranks, (suit, rank) => new Card(rank, suit));
}