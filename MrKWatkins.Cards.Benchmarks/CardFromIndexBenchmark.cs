using System.Diagnostics.Contracts;
using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks;

[MemoryDiagnoser]
public class CardFromIndexBenchmark
{
    private static readonly IReadOnlyList<Card> FullDeck = Card.FullDeck.ToArray();
    
    private static readonly IReadOnlyList<Rank> RankFromIndex = new[]
    {
        Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King,
        Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King,
        Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King,
        Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King
    };

    private static readonly IReadOnlyList<Suit> SuitFromIndex = new[]
    {
        Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades, Suit.Spades,
        Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts, Suit.Hearts,
        Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds, Suit.Diamonds,
        Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs, Suit.Clubs
    };
    
    [Benchmark(Baseline = true)]
    public Card[] ModAndDivide() => RunTest(ModAndDivide);

    [Benchmark]
    public Card[] RankAndSuitLookup() => RunTest(RankAndSuitLookup);

    [Benchmark]
    public Card[] CardLookup() => RunTest(CardLookup);

    [Pure]
    private static Card[] RunTest(Func<int, Card> function)
    {
        var result = new Card[52];
        for (var f = 0; f < 52; f++)
        {
            result[f] = function(f);
        }

        return result;
    }

    [Pure]
    private static Card ModAndDivide(int index) => new ((Rank)(index % 13), (Suit)(index / 13));

    [Pure]
    private static Card RankAndSuitLookup(int index) => new(RankFromIndex[index], SuitFromIndex[index]);

    [Pure]
    private static Card CardLookup(int index) => FullDeck[index];
}