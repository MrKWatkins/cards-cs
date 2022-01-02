using System.Diagnostics.Contracts;
using System.Numerics;
using MrKWatkins.Cards.Text;

namespace MrKWatkins.Cards;

public readonly struct Card : IEquatable<Card>
{
    public Card(Rank rank, Suit suit)
    {
        Suit = suit;
        Rank = rank;
    }
    
    public Rank Rank { get; }

    public Suit Suit { get; }
    
    /// <summary>
    /// Gets the zero-based index of the card in a full deck, suit and rank ordered.
    /// </summary>
    internal int Index => (int)Suit * 13 + (int)Rank;

    [Pure]
    internal static Card FromIndex(int index) => new ((Rank)(index % 13), (Suit)(index / 13));

    /// <summary>
    /// Gets a ulong with a single bit set at position <see cref="Index"/>.
    /// </summary>
    internal ulong BitIndex => 1UL << Index;
    
    [Pure]
    internal static Card FromBitIndex(ulong bitIndex) => FromIndex(BitOperations.TrailingZeroCount(bitIndex));

    public bool Equals(Card other) => Suit == other.Suit && Rank == other.Rank;

    public override bool Equals(object? obj) => obj is Card other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Suit, Rank);

    public static bool operator ==(Card left, Card right) => left.Equals(right);

    public static bool operator !=(Card left, Card right) => !left.Equals(right);

    public override string ToString() => CardFormat.Default.Format(this);

    public static IReadOnlyList<Rank> Ranks { get; } = Enum.GetValues<Rank>();
        
    public static IReadOnlyList<Suit> Suits { get; } = Enum.GetValues<Suit>();

    /// <summary>
    /// Returns a full deck of cards in suit then rank order, i.e AS, 2S, 3S..., KS, AH, 2H, etc.
    /// </summary>
    [Pure]
    public static IEnumerable<Card> FullDeck()
    {
        for (var suit = 0; suit < 4; suit++)
        {
            for (var rank = 0; rank < 13; rank++)
            {
                yield return new Card((Rank) rank, (Suit) suit);
            }
        }
    }
}
