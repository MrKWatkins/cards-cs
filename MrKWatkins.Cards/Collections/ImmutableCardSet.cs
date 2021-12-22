using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace MrKWatkins.Cards.Collections;

public readonly struct ImmutableCardSet : IImmutableSet<Card>, IReadOnlyCardSet
{
    public static readonly ImmutableCardSet Empty = new (0UL);

    public static readonly ImmutableCardSet FullDeck = new (BitIndexOperations.FullDeckBitIndices);
    
    private readonly ulong bitIndices;
    
    private ImmutableCardSet(ulong bitIndices)
    {
        this.bitIndices = bitIndices;
    }

    public ImmutableCardSet(params Card[] cards)
        : this((IEnumerable<Card>)cards)
    {
    }

    public ImmutableCardSet(IReadOnlyCardSet cardSet)
        : this(cardSet.BitIndices)
    {
    }

    public ImmutableCardSet([JetBrains.Annotations.InstantHandle] IEnumerable<Card> cards)
        : this(BitIndexOperations.ToBitIndices(cards))
    {
    }
    
    ulong IReadOnlyCardSet.BitIndices => bitIndices;

    [Pure]
    public IImmutableSet<Card> Add(Card card) => new ImmutableCardSet(BitIndexOperations.Union(bitIndices, card.BitIndex));

    [Pure]
    public IImmutableSet<Card> Clear() => Empty;
    
    [Pure]
    public bool Contains(Card card) => BitIndexOperations.Contains(bitIndices, card.BitIndex);

    public int Count => BitOperations.PopCount(bitIndices);

    [Pure]
    public IImmutableSet<Card> Remove(Card card) => new ImmutableCardSet(BitIndexOperations.Except(bitIndices, card.BitIndex));

    [Pure]
    public IEnumerator<Card> GetEnumerator() => new BitIndexEnumerator(bitIndices);

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

    [Pure]
    private IImmutableSet<Card> Except(ulong other) => new ImmutableCardSet(BitIndexOperations.Except(bitIndices, other));

    [Pure]
    public IImmutableSet<Card> Except(IReadOnlyCardSet other) => Except(other.BitIndices);

    [Pure]
    public IImmutableSet<Card> Except(IEnumerable<Card> other) => Except(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private IImmutableSet<Card> Intersect(ulong other) => new ImmutableCardSet(BitIndexOperations.Intersect(bitIndices, other));

    [Pure]
    public IImmutableSet<Card> Intersect(IReadOnlyCardSet other) => Intersect(other.BitIndices);

    [Pure]
    public IImmutableSet<Card> Intersect(IEnumerable<Card> other) => Intersect(BitIndexOperations.ToBitIndices(other));

    [Pure]
    private IImmutableSet<Card> SymmetricExcept(ulong other) => new ImmutableCardSet(BitIndexOperations.SymmetricExcept(bitIndices, other));

    [Pure]
    public IImmutableSet<Card> SymmetricExcept(IReadOnlyCardSet other) => SymmetricExcept(other.BitIndices);

    [Pure]
    public IImmutableSet<Card> SymmetricExcept(IEnumerable<Card> other) => SymmetricExcept(BitIndexOperations.ToBitIndices(other));

    [Pure]
    public bool TryGetValue(Card equalValue, out Card actualValue)
    {
        actualValue = equalValue;
        return Contains(equalValue);
    }
    
    [Pure]
    private IImmutableSet<Card> Union(ulong other) => new ImmutableCardSet(BitIndexOperations.Union(bitIndices, other));

    [Pure]
    public IImmutableSet<Card> Union(IReadOnlyCardSet other) => Union(other.BitIndices);

    [Pure]
    public IImmutableSet<Card> Union(IEnumerable<Card> other) => Union(BitIndexOperations.ToBitIndices(other));
}