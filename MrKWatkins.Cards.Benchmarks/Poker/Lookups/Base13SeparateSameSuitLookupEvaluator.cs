using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Poker;

namespace MrKWatkins.Cards.Benchmarks.Poker.Lookups;

public sealed class Base13SeparateSameSuitLookupEvaluator
{
    public static readonly Base13SeparateSameSuitLookupEvaluator Instance = Build();
    private readonly PokerHand[] lookup;

    private Base13SeparateSameSuitLookupEvaluator(PokerHand[] lookup)
    {
        this.lookup = lookup;
    }

    [Pure]
    private static Base13SeparateSameSuitLookupEvaluator Build()
    {
        // 5 x 13 for ranks. For same suit we use one 13 bit number = 8192.
        var lookup = new PokerHand[371293 + 8192];
        var evaluator = new PokerEvaluator();
        foreach (var hand in Card.FullDeck.Combinations(5))
        {
            var key = GetKey(hand);
            lookup[key] = evaluator.EvaluateFiveCardHand(hand);
        }

        return new Base13SeparateSameSuitLookupEvaluator(lookup);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int GetKey(IEnumerable<Card> hand)
    {
        var suit = 0;
        
        using var enumerator = hand.GetEnumerator();
        
        enumerator.MoveNext();
        var card0 = enumerator.Current;
        suit |= 1 << (int)card0.Suit;
        
        enumerator.MoveNext();
        var card1 = enumerator.Current;
        suit |= 1 << (int)card1.Suit;
        
        enumerator.MoveNext();
        var card2 = enumerator.Current;
        suit |= 1 << (int)card2.Suit;
        
        enumerator.MoveNext();
        var card3 = enumerator.Current;
        suit |= 1 << (int)card3.Suit;
        
        enumerator.MoveNext();
        var card4 = enumerator.Current;
        suit |= 1 << (int)card4.Suit;

        // Reset the lowest set bit; if we only had one suit we only had one bit. Marginally faster than checking the pop count is 0.
        if ((suit & (suit - 1)) == 0)
        {
            var rankOr = (1 << (int)card0.Rank) |
                         (1 << (int)card1.Rank) |
                         (1 << (int)card2.Rank) |
                         (1 << (int)card3.Rank) |
                         (1 << (int)card4.Rank);
            return 371293 + rankOr;
        }

        return (int)card0.Rank +
               (int)card1.Rank * 13 +
               (int)card2.Rank * 13 * 13 +
               (int)card3.Rank * 13 * 13 * 13 +
               (int)card4.Rank * 13 * 13 * 13 * 13;
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PokerHand EvaluateFiveCardHand([JetBrains.Annotations.InstantHandle] IEnumerable<Card> hand) => 
        lookup[GetKey(hand)];
}