using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Poker;

namespace MrKWatkins.Cards.Benchmarks.Poker;

[MemoryDiagnoser]
public class EvaluateFiveCardHandBenchmark
{
    private readonly IReadOnlyList<IReadOnlyCardSet> allFiveCardHands = Card.FullDeck.Combinations(5).ToList();
    private readonly PokerEvaluator evaluator = new ();

    [Benchmark(Baseline = true)]
    public void EvaluateFiveCardHand()
    {
        foreach (var hand in allFiveCardHands)
        {
            var _ = evaluator.EvaluateFiveCardHand(hand);
        }
    }
}