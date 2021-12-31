using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Text;

public static class SuitFormatter
{
    public static IFormatter<Suit> Default => CreateUpperCaseLetter();
    
    /// <summary>
    /// Creates a formatter that formats as a single upper case letter, e.g. Spades = S, Diamonds = D.
    /// </summary>
    [Pure]
    public static IFormatter<Suit> CreateUpperCaseLetter() => new EnumFormatter<Suit>("S", "H", "D", "C");
    
    /// <summary>
    /// Creates a formatter that formats as a single lower case letter, e.g. Spades = s, Diamonds = d.
    /// </summary>
    [Pure]
    public static IFormatter<Suit> CreateLowerCaseLetter() => new EnumFormatter<Suit>("s", "h", "d", "c");
    
    /// <summary>
    /// Creates a formatter that formats as the Unicode black symbol, e.g. Spades = ♠, Diamonds = ♦.
    /// </summary>
    [Pure]
    public static IFormatter<Suit> CreateBlackSymbol() => new EnumFormatter<Suit>("\u2660", "\u2665", "\u2666", "\u2663");
    
    /// <summary>
    /// Creates a formatter that formats as the Unicode white symbol, e.g. Spades = ♤, Diamonds = ♢.
    /// </summary>
    [Pure]
    public static IFormatter<Suit> CreateWhiteSymbol() => new EnumFormatter<Suit>("\u2664", "\u2661", "\u2662", "\u2667");
    
    /// <summary>
    /// Creates a formatter that formats using the full English word in title case, E.g. Spades = Spades, Diamonds = Diamonds.
    /// </summary>
    [Pure]
    public static IFormatter<Suit> CreateTitleCaseWord() => new EnumFormatter<Suit>("Spades", "Hearts", "Diamonds", "Clubs");
    
    /// <summary>
    /// Creates a formatter that formats using the full English word in lower case, E.g. Spades = spades, Diamonds = diamonds.
    /// </summary>
    [Pure]
    public static IFormatter<Suit> CreateLowerCaseWord() => new EnumFormatter<Suit>("spades", "hearts", "diamonds", "clubs");
}