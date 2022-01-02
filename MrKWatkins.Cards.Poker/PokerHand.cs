using System.Numerics;

namespace MrKWatkins.Cards.Poker;

/// <summary>
/// Represents a 5 card poker hand.
/// </summary>
/// <remarks>
/// Coded internally as a 32-bit int comprising two rank masks and bits describing the hand type.
/// 
/// *  0 -> 12 - Secondary rank mask. Ace always high for secondary so we don't include a bit for the low ace.
/// * 13 -> 26 - Primary rank mask. Ace can be high (e.g. four of a kind) or low (e.g. ace to five straight) so have bits for both low and high aces.
/// * 27 -> 30 - Hand type. 11 possible hands so can be encoded in four bits.
/// 
/// Bit 31 is not used so all numbers will be positive. Better hands are represented by higher numbers.
/// </remarks>
public sealed class PokerHand : IComparable<PokerHand>, IEquatable<PokerHand>
{
    private const int HandTypeMask = 0x78000000;        // 1s at bits 27 -> 30.
    private const int PrimaryRankMask = 0x7FFE000;      // 1s at bits 13 -> 26. Should really be PrimaryRankMaskMask...
    private const int SecondaryRankMask = 0x1FFF;       // 1s at bits 0 -> 12.

    private readonly int hand;

    internal PokerHand(PokerHandType handType, ulong primaryRankMask, ulong secondaryRankMask = 0)
    {
        hand = ((int) handType << 27) |
               ((int) primaryRankMask << 13) |
               ((int) secondaryRankMask >> 1);   // Shift left 1 as we don't store a low ace bit.
    }

    public PokerHandType Type => (PokerHandType) ((hand & HandTypeMask) >> 27);

    public IEnumerable<Rank> PrimaryRanks => RankMaskToRanks((hand & PrimaryRankMask) >> 13).Reverse();

    public IEnumerable<Rank> SecondaryRanks => RankMaskToRanks((hand & SecondaryRankMask) << 1).Reverse(); // Shift left 1 as we don't store a low ace bit.

    public int CompareTo(PokerHand? other)
    {
        if (other is null)
        {
            return +1;
        }

        return hand.CompareTo(other.hand);
    }

    public bool Equals(PokerHand? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return hand == other.hand;
    }

    public override bool Equals(object? obj) => Equals(obj as PokerHand);

    public override int GetHashCode() => hand;

    public static bool operator ==(PokerHand? left, PokerHand? right) => Equals(left, right);

    public static bool operator !=(PokerHand? left, PokerHand? right) => !Equals(left, right);
    
    private static IEnumerable<Rank> RankMaskToRanks(int rankMask)
    {
        while (rankMask != 0)
        {
            var rightMostBit = rankMask & -rankMask;
            yield return RankMaskToRank((short) rightMostBit);

            rankMask ^= rightMostBit;
        }
    }

    private static Rank RankMaskToRank(short rankMask)
    {
        if (rankMask == (short) PokerEvaluation.AceHighRankMask)
        {
            return Rank.Ace;
        }

        return (Rank) RankMaskToRankNumber(rankMask);
    }

    private static int RankMaskToRankNumber(int rankMask) => BitOperations.TrailingZeroCount(rankMask);
}