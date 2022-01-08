using System.Collections;
using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards;

internal static class BitEnumerationExtensions
{
    /// <summary>
    /// Enumerates over the set bits in a <see cref="ulong" />, returning a ulong for each set bit with just that bit set.
    /// </summary>
    [Pure]
    public static IEnumerable<ulong> EnumerateSetBits(this ulong value) => new BitEnumerator(value);
    
    private struct BitEnumerator : IEnumerable<ulong>, IEnumerator<ulong>
    {
        private const ulong NotStarted = ulong.MaxValue;
        private readonly ulong start;
        private ulong remaining;

        internal BitEnumerator(ulong start)
        {
            this.start = start;
            remaining = NotStarted;
            Current = default;
        }

        public bool MoveNext()
        {
            remaining = remaining == NotStarted ? start : remaining.ResetLowestSetBit();
            
            if (remaining == 0UL)
            {
                return false;
            }
            
            Current = remaining.ExtractLowestSetBit();
            return true;
        }

        public ulong Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Reset()
        {
            remaining = NotStarted;
            Current = default;
        }

        public void Dispose()
        {
        }

        public IEnumerator<ulong> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}