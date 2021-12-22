using System.Collections;

namespace MrKWatkins.Cards.Collections;

internal struct BitIndexEnumerator : IEnumerator<Card>
{
    private const ulong NotStarted = ulong.MaxValue;
    private readonly ulong startBitIndices;
    private ulong bitIndices;
    private Card current;

    internal BitIndexEnumerator(ulong bitIndices)
    {
        startBitIndices = bitIndices;
        this.bitIndices = NotStarted;
        current = default;
    }

    public bool MoveNext()
    {
        bitIndices = bitIndices == NotStarted ? startBitIndices : bitIndices.ResetRightmostBit();
            
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
        bitIndices = NotStarted;
        current = default;
    }
}