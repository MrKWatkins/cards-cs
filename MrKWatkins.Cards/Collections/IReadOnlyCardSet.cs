using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Collections;

public interface IReadOnlyCardSet : IReadOnlySet<Card>
{
    internal ulong BitIndices { get; }

    [Pure]
    public bool IsProperSubsetOf(IReadOnlyCardSet other);

    [Pure]
    public bool IsProperSupersetOf(IReadOnlyCardSet other);

    [Pure]
    public bool IsSubsetOf(IReadOnlyCardSet other);

    [Pure]
    public bool IsSupersetOf(IReadOnlyCardSet other);

    [Pure]
    public bool Overlaps(IReadOnlyCardSet other);

    [Pure]
    public bool SetEquals(IReadOnlyCardSet other);
}