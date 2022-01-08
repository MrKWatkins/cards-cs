using System.Collections;

namespace MrKWatkins.Cards.Collections;

internal struct CardEnumerator : IEnumerator<Card>
{
    private const ulong NotStarted = ulong.MaxValue;
    private readonly ulong startBitIndices;
    private ulong bitIndices;
    private Card current;

    internal CardEnumerator(ulong bitIndices)
    {
        startBitIndices = bitIndices;
        this.bitIndices = NotStarted;
        current = default;
    }

    public bool MoveNext()
    {
        bitIndices = bitIndices == NotStarted ? startBitIndices : bitIndices.ResetLowestSetBit();
            
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
        bitIndices = NotStarted;
        current = default;
    }
}