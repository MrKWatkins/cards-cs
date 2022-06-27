using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MrKWatkins.Cards.Collections;

namespace MrKWatkins.Cards.Poker;

public sealed class LookupEvaluator2
{
    public static readonly LookupEvaluator2 Instance = Build();
    private readonly PokerHand[] lookup;

    private LookupEvaluator2(PokerHand[] lookup)
    {
        this.lookup = lookup;
    }

    [Pure]
    private static LookupEvaluator2 Build()
    {
        var lookup = new PokerHand[742586];
        var evaluator = new PokerEvaluator();
        foreach (var hand in Card.FullDeck.Combinations(5))
        {
            var key = GetKey(hand);
            lookup[key] = evaluator.EvaluateFiveCardHand(hand);
        }

        return new LookupEvaluator2(lookup);
    }

    [Pure]
    private static int GetKey(IEnumerable<Card> hand)
    {
        var number = 0;
        var suit = 0;
        foreach (var card in hand)
        {
            number *= 13;
            number += (int)card.Rank;
            suit |= 1 << (int)card.Suit;
        }

        // Reset the lowest set bit; if we only had one suit we only had one bit. Marginally faster than checking the pop count is 0.
        number = (suit & (suit - 1)) == 0 ? number + 371293 : number;

        return number;
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PokerHand EvaluateFiveCardHand([JetBrains.Annotations.InstantHandle] IEnumerable<Card> hand) => 
        lookup[GetKey(hand)];
}