using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Poker;

public interface IPokerEvaluator
{
    [Pure]
    PokerHand EvaluateFiveCardHand([JetBrains.Annotations.InstantHandle] IEnumerable<Card> hand);
    
    [Pure]
    PokerHand EvaluateFiveCardHand(IReadOnlyCollection<Card> hand);
    
    [Pure]
    PokerHand EvaluateSevenCardHand([JetBrains.Annotations.InstantHandle] IEnumerable<Card> hand);
    
    [Pure]
    PokerHand EvaluateSevenCardHand(IReadOnlyCollection<Card> hand);
}