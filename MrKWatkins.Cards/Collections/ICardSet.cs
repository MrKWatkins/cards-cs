namespace MrKWatkins.Cards.Collections;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface ICardSet : ISet<Card>, IReadOnlyCardSet
{
    new bool Clear();
    
    void ExceptWith(IReadOnlyCardSet other);
    
    void IntersectWith(IReadOnlyCardSet other);
    
    void SymmetricExceptWith(IReadOnlyCardSet other);
    
    void UnionWith(IReadOnlyCardSet other);
}