using System.Collections;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace MrKWatkins.Cards.Collections;

public abstract class MutableCardSet : ICardSet
{
    private protected MutableCardSet()
    {
    }

    private protected MutableCardSet(ulong bitIndices)
    {
        BitIndices = bitIndices;
    }

    private protected MutableCardSet(params Card[] cards)
        : this((IEnumerable<Card>)cards)
    {
    }

    private protected MutableCardSet(IReadOnlyCardSet cardSet)
        : this(cardSet.BitIndices)
    {
    }

    private protected MutableCardSet([JetBrains.Annotations.InstantHandle] IEnumerable<Card> cards)
        : this(BitIndexOperations.ToBitIndices(cards))
    {
    }

    ulong IReadOnlyCardSet.BitIndices => BitIndices;
    
    private protected ulong BitIndices { get; set; }
    
    // operation takes BitIndices and an operand to avoid having to create a closure type containing the operand. Reduces allocations to 0 and is about 4 times quicker.
    private protected abstract bool Mutate(Func<ulong, ulong, ulong> operation, ulong operand);
    
    void ICollection<Card>.Add(Card card) => Add(card);

    public bool Add(Card card) => Mutate(BitIndexOperations.Union, card.BitIndex);

    void ICollection<Card>.Clear() => Clear();

    public bool Clear() => Mutate((_, _) => 0UL, 0UL);

    public bool Contains(Card card) => BitIndexOperations.Contains(BitIndices, card.BitIndex);

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

    public int Count => BitOperations.PopCount(BitIndices);

    public bool IsReadOnly => false;

    public bool Remove(Card card) => Mutate(BitIndexOperations.Except, card.BitIndex);

    private void ExceptWith(ulong otherBitIndices) => Mutate(BitIndexOperations.Except, otherBitIndices);

    public void ExceptWith(IReadOnlyCardSet other) => ExceptWith(other.BitIndices);

    public void ExceptWith(IEnumerable<Card> other) => ExceptWith(BitIndexOperations.ToBitIndices(other));

    private void IntersectWith(ulong otherBitIndices) => Mutate(BitIndexOperations.Intersect, otherBitIndices);
    
    public void IntersectWith(IReadOnlyCardSet other) => IntersectWith(other.BitIndices);

    public void IntersectWith(IEnumerable<Card> other) => IntersectWith(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsProperSubsetOf(ulong other) => BitIndexOperations.IsProperSubsetOf(BitIndices, other);
    
    public bool IsProperSubsetOf(IReadOnlyCardSet other) => IsProperSubsetOf(other.BitIndices);

    public bool IsProperSubsetOf(IEnumerable<Card> other) => IsProperSubsetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsProperSupersetOf(ulong other) => BitIndexOperations.IsProperSupersetOf(BitIndices, other);

    public bool IsProperSupersetOf(IReadOnlyCardSet other) => IsProperSupersetOf(other.BitIndices);

    public bool IsProperSupersetOf(IEnumerable<Card> other) => IsProperSupersetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsSubsetOf(ulong other) => BitIndexOperations.IsSubsetOf(BitIndices, other);

    public bool IsSubsetOf(IReadOnlyCardSet other) => IsSubsetOf(other.BitIndices);

    public bool IsSubsetOf(IEnumerable<Card> other) => IsSubsetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool IsSupersetOf(ulong other) => BitIndexOperations.IsSupersetOf(BitIndices, other);
    
    public bool IsSupersetOf(IReadOnlyCardSet other) => IsSupersetOf(other.BitIndices);

    public bool IsSupersetOf(IEnumerable<Card> other) => IsSupersetOf(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool Overlaps(ulong otherBitIndices) => BitIndexOperations.Overlaps(BitIndices, otherBitIndices);
    
    public bool Overlaps(IReadOnlyCardSet other) => Overlaps(other.BitIndices);

    public bool Overlaps(IEnumerable<Card> other) => Overlaps(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private bool SetEquals(ulong otherBitIndices) => BitIndices == otherBitIndices;

    public bool SetEquals(IReadOnlyCardSet other) => SetEquals(other.BitIndices);

    public bool SetEquals(IEnumerable<Card> other) => SetEquals(BitIndexOperations.ToBitIndices(other));

    private void SymmetricExceptWith(ulong otherBitIndices) => Mutate(BitIndexOperations.SymmetricExcept, otherBitIndices);

    public void SymmetricExceptWith(IReadOnlyCardSet other) => SymmetricExceptWith(other.BitIndices);

    public void SymmetricExceptWith(IEnumerable<Card> other) => SymmetricExceptWith(BitIndexOperations.ToBitIndices(other));

    private void UnionWith(ulong otherBitIndices) => Mutate(BitIndexOperations.Union, otherBitIndices);

    public void UnionWith(IReadOnlyCardSet other) => UnionWith(other.BitIndices);

    public void UnionWith(IEnumerable<Card> other) => UnionWith(BitIndexOperations.ToBitIndices(other));

    public abstract IEnumerator<Card> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}