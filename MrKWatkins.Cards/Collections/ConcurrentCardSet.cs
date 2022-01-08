using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Collections;

/// <summary>
/// A thread-safe, unique mutable set of <see cref="Card"/>s.
/// </summary>
public sealed class ConcurrentCardSet : MutableCardSet
{
    private SpinLock @lock;

    public ConcurrentCardSet()
    {
    }

    internal ConcurrentCardSet(ulong bitIndices)
        : base(bitIndices)
    {
    }

    public ConcurrentCardSet(params Card[] cards)
        : base(cards)
    {
    }

    public ConcurrentCardSet(IReadOnlyCardSet cardSet)
        : base(cardSet)
    {
    }

    public ConcurrentCardSet([JetBrains.Annotations.InstantHandle] IEnumerable<Card> cards)
        : base(cards)
    {
    }

    [Pure]
    public static ConcurrentCardSet CreateFullDeck() => new (BitIndexOperations.FullDeckBitIndices);

    private protected override bool Mutate(Func<ulong, ulong, ulong> operation, ulong operand)
    {
        var lockTaken = false;
        @lock.Enter(ref lockTaken);
        
        // Not checking lockTaken as it should always be true given thread ownership tracking is disabled.

        // Not worrying about torn reads here or elsewhere as we only target x64.
        var before = BitIndices;
        BitIndices = operation(BitIndices, operand);
        var result = before != BitIndices;
        @lock.Exit();
        return result;
    }

    public override IEnumerator<Card> GetEnumerator() => new CardEnumerator(BitIndices);
}