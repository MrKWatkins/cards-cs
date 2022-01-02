using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Poker;

namespace MrKWatkins.Cards.Benchmarks.Poker;

[MemoryDiagnoser]
public class EvaluateAllBenchmark
{
    private readonly IReadOnlyList<IReadOnlyCardSet> allFiveCardHands = Card.FullDeck.Combinations(5).ToList();

    [Benchmark(Baseline = true)]
    public void Evaluate()
    {
        foreach (var hand in allFiveCardHands)
        {
            PokerEvaluation.Evaluate(hand);
        }
    }
}