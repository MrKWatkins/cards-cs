namespace MrKWatkins.Cards;

/// <summary>
/// The rank of a card.
/// </summary>
/// <remarks>
/// The values of this enum are slightly odd, in that ace is 0, two is 1, etc. The reason for this is partly to avoid having a default
/// value that isn't an actual rank, and partly to avoid various - 1s in the code base. Plus you shouldn't need to use the underlying
/// number anyway of course...
/// </remarks>
public enum Rank
{        
    Ace = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8,
    Ten = 9,
    Jack = 10,
    Queen = 11,
    King = 12
}
