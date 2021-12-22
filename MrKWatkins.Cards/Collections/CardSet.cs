using System.Collections;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace MrKWatkins.Cards.Collections;

/// <summary>
/// A unique mutable set of <see cref="Card"/>s.
/// </summary>
public sealed class CardSet : ICardSet
{
    private ulong bitIndices;
    private int version;

    public CardSet()
    {
    }

    private CardSet(ulong bitIndices)
    {
        this.bitIndices = bitIndices;
    }

    public CardSet(params Card[] cards)
        : this((IEnumerable<Card>)cards)
    {
    }

    public CardSet(IReadOnlyCardSet cardSet)
        : this(cardSet.BitIndices)
    {
    }

    public CardSet([JetBrains.Annotations.InstantHandle] IEnumerable<Card> cards)
        : this(BitIndexOperations.ToBitIndices(cards))
    {
    }

    [Pure]
    public static CardSet CreateFullDeck() => new (BitIndexOperations.FullDeckBitIndices);

    ulong IReadOnlyCardSet.BitIndices => bitIndices;
    
    // operation takes bitIndices and an operand to avoid having to create a closure type containing the operand. Reduces allocations to 0 and is about 4 times quicker.
    private bool Mutate(Func<ulong, ulong, ulong> operation, ulong operand)
    {
        var before = bitIndices;
        bitIndices = operation(bitIndices, operand);
        if (before != bitIndices)
        {
            version++;
            return true;
        }

        return false;
    }
    
    void ICollection<Card>.Add(Card card) => Add(card);

    public bool Add(Card card) => Mutate(BitIndexOperations.Union, card.BitIndex);

    void ICollection<Card>.Clear() => Clear();

    public bool Clear() => Mutate((_, _) => 0UL, 0UL);

    [Pure]
    public bool Contains(Card card) => BitIndexOperations.Contains(bitIndices, card.BitIndex);

    public void CopyTo(Card[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Destination array was not long enough. Check the destination index, length, and the array's lower bounds.", nameof(array));
        }

        foreach (var card in this)
        {
            array[arrayIndex] = card;
            arrayIndex++;
        }
    }

    [Pure]
    public int Count => BitOperations.PopCount(bitIndices);

    [Pure]
    public bool IsReadOnly => false;

    public bool Remove(Card card) => Mutate(BitIndexOperations.Except, card.BitIndex);

    [Pure]
    public IEnumerator<Card> GetEnumerator() => new Enumerator(this);

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void ExceptWith(ulong otherBitIndices) => Mutate(BitIndexOperations.Except, otherBitIndices);

    public void ExceptWith(IReadOnlyCardSet other) => ExceptWith(other.BitIndices);

    public void ExceptWith(IEnumerable<Card> other) => ExceptWith(BitIndexOperations.ToBitIndices(other));

    private void IntersectWith(ulong otherBitIndices) => Mutate(BitIndexOperations.Intersect, otherBitIndices);
    
    public void IntersectWith(IReadOnlyCardSet other) => IntersectWith(other.BitIndices);

    public void IntersectWith(IEnumerable<Card> other) => IntersectWith(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsProperSubsetOf(ulong other) => BitIndexOperations.IsProperSubsetOf(bitIndices, other);
    
    [Pure]
    public bool IsProperSubsetOf(IReadOnlyCardSet other) => IsProperSubsetOf(other.BitIndices);

    [Pure]
    public bool IsProperSubsetOf(IEnumerable<Card> other) => IsProperSubsetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsProperSupersetOf(ulong other) => BitIndexOperations.IsProperSupersetOf(bitIndices, other);

    [Pure]
    public bool IsProperSupersetOf(IReadOnlyCardSet other) => IsProperSupersetOf(other.BitIndices);

    [Pure]
    public bool IsProperSupersetOf(IEnumerable<Card> other) => IsProperSupersetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsSubsetOf(ulong other) => BitIndexOperations.IsSubsetOf(bitIndices, other);

    [Pure]
    public bool IsSubsetOf(IReadOnlyCardSet other) => IsSubsetOf(other.BitIndices);

    [Pure]
    public bool IsSubsetOf(IEnumerable<Card> other) => IsSubsetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsSupersetOf(ulong other) => BitIndexOperations.IsSupersetOf(bitIndices, other);
    
    [Pure]
    public bool IsSupersetOf(IReadOnlyCardSet other) => IsSupersetOf(other.BitIndices);

    [Pure]
    public bool IsSupersetOf(IEnumerable<Card> other) => IsSupersetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool Overlaps(ulong otherBitIndices) => BitIndexOperations.Overlaps(bitIndices, otherBitIndices);
    
    [Pure]
    public bool Overlaps(IReadOnlyCardSet other) => Overlaps(other.BitIndices);

    [Pure]
    public bool Overlaps(IEnumerable<Card> other) => Overlaps(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool SetEquals(ulong otherBitIndices) => bitIndices == otherBitIndices;

    [Pure]
    public bool SetEquals(IReadOnlyCardSet other) => SetEquals(other.BitIndices);

    [Pure]
    public bool SetEquals(IEnumerable<Card> other) => SetEquals(BitIndexOperations.ToBitIndices(other));

    private void SymmetricExceptWith(ulong otherBitIndices) => Mutate(BitIndexOperations.SymmetricExcept, otherBitIndices);

    public void SymmetricExceptWith(IReadOnlyCardSet other) => SymmetricExceptWith(other.BitIndices);

    public void SymmetricExceptWith(IEnumerable<Card> other) => SymmetricExceptWith(BitIndexOperations.ToBitIndices(other));

    private void UnionWith(ulong otherBitIndices) => Mutate(BitIndexOperations.Union, otherBitIndices);

    public void UnionWith(IReadOnlyCardSet other) => UnionWith(other.BitIndices);

    public void UnionWith(IEnumerable<Card> other) => UnionWith(BitIndexOperations.ToBitIndices(other));

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

            bitIndices = bitIndices == NotStarted ? cardSet.bitIndices : bitIndices.ResetRightmostBit();
            
            if (bitIndices == 0UL)
            {
                return false;
            }
            
            current = Card.FromBitIndex(bitIndices.ExtractRightmostBit());
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