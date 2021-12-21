namespace MrKWatkins.Cards.Collections;

public interface IReadOnlyCardSet : IReadOnlySet<Card>
{
    internal ulong BitIndices { get; }
}