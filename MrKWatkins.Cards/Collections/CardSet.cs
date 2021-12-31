using System.Collections;
using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Collections;

/// <summary>
/// A unique mutable set of <see cref="Card"/>s.
/// </summary>
public sealed class CardSet : MutableCardSet
{
    private int version;

    public CardSet()
    {
    }

    private CardSet(ulong bitIndices)
        : base(bitIndices)
    {
    }

    public CardSet(params Card[] cards)
        : base(cards)
    {
    }

    public CardSet(IReadOnlyCardSet cardSet)
        : base(cardSet)
    {
    }

    public CardSet([JetBrains.Annotations.InstantHandle] IEnumerable<Card> cards)
        : base(cards)
    {
    }

    [Pure]
    public static CardSet CreateFullDeck() => new (BitIndexOperations.FullDeckBitIndices);

    private protected override bool Mutate(Func<ulong, ulong, ulong> operation, ulong operand)
    {
        var before = BitIndices;
        BitIndices = operation(BitIndices, operand);
        if (before != BitIndices)
        {
            version++;
            return true;
        }

        return false;
    }
    
    public override IEnumerator<Card> GetEnumerator() => new Enumerator(this);

    private struct Enumerator : IEnumerator<Card>
    {
        private const ulong NotStarted = ulong.MaxValue;
        private readonly CardSet cardSet;
        private readonly int version;
        private ulong bitIndices;
        private Card current;

        internal Enumerator(CardSet cardSet)
        {
            this.cardSet = cardSet;
            version = cardSet.version;
            bitIndices = NotStarted;
            current = default;
        }

        public bool MoveNext()
        {
            ValidateVersion();

            bitIndices = bitIndices == NotStarted ? cardSet.BitIndices : bitIndices.ResetLowestSetBit();
            
            if (bitIndices == 0UL)
            {
                return false;
            }
            
            current = Card.FromBitIndex(bitIndices.ExtractLowestSetBit());
            return true;
        }

        public void Dispose()
        {
        }

        public Card Current
        {
            get
            {
                if (bitIndices is ulong.MaxValue or 0UL)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }

                return current;
            }
        }

        object IEnumerator.Current => Current;

        public void Reset()
        {
            ValidateVersion();

            bitIndices = NotStarted;
            current = default;
        }

        private void ValidateVersion()
        {
            if (version != cardSet.version)
            {
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
            }
        }
    }
}