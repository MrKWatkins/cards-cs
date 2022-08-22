using BenchmarkDotNet.Attributes;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Poker;

namespace MrKWatkins.Cards.Benchmarks.Poker;

[MemoryDiagnoser]
public class EvaluateFiveCardHandBenchmark
{
    private readonly IReadOnlyList<IReadOnlyCardSet> allFiveCardHands = Card.FullDeck.Combinations(5).ToList();
    private readonly PokerEvaluator pokerEvaluator = new ();

    [Benchmark(Baseline = true)]
    public void PokerEvaluator()
    {
        foreach (var hand in allFiveCardHands)
        {
            var _ = pokerEvaluator.EvaluateFiveCardHand(hand);
        }
    }
    
    [Benchmark]
    public void BitRepresentationLookupEvaluator()
    {
        foreach (var hand in allFiveCardHands)
        {
            var _ = Lookups.BitRepresentationLookupEvaluator.Instance.EvaluateFiveCardHand(hand);
        }
    }
    
    [Benchmark]
    public void Base13LookupEvaluator()
    {
        foreach (var hand in allFiveCardHands)
        {
            var _ = Lookups.Base13LookupEvaluator.Instance.EvaluateFiveCardHand(hand);
        }
    }
    
    [Benchmark(Baseline = true)]
    public void Base13SeparateSameSuitLookupEvaluator()
    {
        foreach (var hand in allFiveCardHands)
        {
            var _ = Lookups.Base13SeparateSameSuitLookupEvaluator.Instance.EvaluateFiveCardHand(hand);
        }
    }
    
    [Benchmark]
    public void IntrinsicsBase13SeparateSameSuitLookupEvaluator()
    {
        foreach (var hand in allFiveCardHands)
        {
            var _ = Lookups.IntrinsicsBase13SeparateSameSuitLookupEvaluator.Instance.EvaluateFiveCardHand(hand);
        }
    }
}