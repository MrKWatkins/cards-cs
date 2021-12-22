namespace MrKWatkins.Cards.Collections;

public interface IReadOnlyCardSet : IReadOnlySet<Card>
{
    internal ulong BitIndices { get; }

    public bool IsProperSubsetOf(IReadOnlyCardSet other);

    public bool IsProperSupersetOf(IReadOnlyCardSet other);

    public bool IsSubsetOf(IReadOnlyCardSet other);

    public bool IsSupersetOf(IReadOnlyCardSet other);

    public bool Overlaps(IReadOnlyCardSet other);

    public bool SetEquals(IReadOnlyCardSet other);
}