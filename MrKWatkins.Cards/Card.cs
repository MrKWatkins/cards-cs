using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using MrKWatkins.Cards.Text;

namespace MrKWatkins.Cards;

// TODO: ISpanFormattable, IFormattable
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
    internal static Card FromIndex(int index) => FullDeck[index];

    /// <summary>
    /// Gets a ulong with a single bit set at position <see cref="Index"/>.
    /// </summary>
    internal ulong BitIndex => 1UL << Index;
    
    [Pure]
    internal static Card FromBitIndex(ulong bitIndex) => FromIndex(BitOperations.TrailingZeroCount(bitIndex));

    /// <summary>
    /// Gets a ulong of the card encoded with a single set bit. Divide the 64 bits of a ulong into 4, 16 for each suit.
    /// The rank is then encoded as the set bit from 0 -> 12 in the relevant 16 bits.
    /// </summary>
    internal ulong BitMask => 1UL << (int) Rank << ((int) Suit * 16);

    [Pure]
    internal static Card FromBitMask(ulong bitMask) => 
        FromBitIndex(Bmi2.X64.IsSupported ? BitMaskToBitIndex_Intrinsic(bitMask) : BitMaskToBitIndex_Fallback(bitMask));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong BitMaskToBitIndex_Intrinsic(ulong bitMask)
    {
        // Bit masks: Spades = 0 -> 12, Hearts = 16 -> 28, Diamonds = 32 -> 44, Clubs = 48 -> 60.
        // Bit indices: Spades = 0 -> 12, Hearts = 13 -> 25, Diamonds = 26 -> 38, Clubs = 39 -> 51.

        // _pext_u64 extracts bits according to a mask and places them in the contiguous low bits.
        // https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html#text=_pext_u64
        const ulong mask = 0b0001111111111111_0001111111111111_0001111111111111_0001111111111111;
        return Bmi2.X64.ParallelBitExtract(bitMask, mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong BitMaskToBitIndex_Fallback(ulong bitMask)
    {
        // Bit indices: Spades = 0 -> 12, Hearts = 13 -> 25, Diamonds = 26 -> 38, Clubs = 39 -> 51.
        // Bit masks: Spades = 0 -> 12, Hearts = 16 -> 28, Diamonds = 32 -> 44, Clubs = 48 -> 60.
        var spades = bitMask & 0x000000000000FFFF;
        var hearts = (bitMask & 0x00000000FFFF0000) >> 3;
        var diamonds = (bitMask & 0x0000FFFF00000000) >> 6;
        var clubs = (bitMask & 0xFFFF000000000000) >> 9;
        return spades | hearts | diamonds | clubs;
    }
    
    public bool Equals(Card other) => Suit == other.Suit && Rank == other.Rank;

    public override bool Equals(object? obj) => obj is Card other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Suit, Rank);

    public static bool operator ==(Card left, Card right) => left.Equals(right);

    public static bool operator !=(Card left, Card right) => !left.Equals(right);

    public override string ToString() => CardFormat.Default.Format(this);

    public static IReadOnlyList<Rank> Ranks { get; } = Enum.GetValues<Rank>();
        
    public static IReadOnlyList<Suit> Suits { get; } = Enum.GetValues<Suit>();

    /// <summary>
    /// A full deck of cards in suit then rank order, i.e AS, 2S, 3S..., KS, AH, 2H, etc.
    /// </summary>
    public static IReadOnlyList<Card> FullDeck { get; } = CreateFullDeck();
    
    [Pure]
    private static IReadOnlyList<Card> CreateFullDeck()
    {
        var fullDeck = new Card[52];
        var index = 0;
        for (var suit = 0; suit < 4; suit++)
        {
            for (var rank = 0; rank < 13; rank++)
            {
                fullDeck[index] = new Card((Rank) rank, (Suit) suit);
                index++;
            }
        }

        return fullDeck;
    }
}
