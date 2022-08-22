using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using MrKWatkins.Cards.Collections;
using MrKWatkins.Cards.Poker;

namespace MrKWatkins.Cards.Benchmarks.Poker.Lookups;

public sealed class BitRepresentationLookupEvaluator
{
    public static readonly BitRepresentationLookupEvaluator Instance = Build();
    private readonly PokerHand[] lookup;

    private BitRepresentationLookupEvaluator(PokerHand[] lookup)
    {
        this.lookup = lookup;
    }

    [Pure]
    private static BitRepresentationLookupEvaluator Build()
    {
        // 5 ranks x 4 bits + 1 bit for same suit.
        var lookup = new PokerHand[1 << 21];
        var evaluator = new PokerEvaluator();
        foreach (var hand in Card.FullDeck.Combinations(5))
        {
            lookup[GetKey(hand)] = evaluator.EvaluateFiveCardHand(hand);
        }

        return new BitRepresentationLookupEvaluator(lookup);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int GetKey(IEnumerable<Card> hand)
    {
        // The key is the 4 bits for each rank then 1 bit for same suit or not.
        var key = 0;
        var position = 0;
        var suit = 0;
        foreach (var card in hand)
        {
            key |= (int)card.Rank << position;
            position += 4;
            suit |= 1 << (int)card.Suit;
        }

        if (position != 20)
        {
            throw new ArgumentException("Value must have 5 cards.", nameof(hand));
        }

        // Reset the lowest set bit; if we only had one suit we only had one bit. Marginally faster than checking the pop count is 0.
        var sameSuit = (suit & (suit - 1)) == 0 ? 1 << 20 : 0;

        return key | sameSuit;
    }
    
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PokerHand EvaluateFiveCardHand([JetBrains.Annotations.InstantHandle] IEnumerable<Card> hand) => lookup[GetKey(hand)];
}