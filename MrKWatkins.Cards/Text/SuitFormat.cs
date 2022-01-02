using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Text;

public static class SuitFormat
{
    public static IFormat<Suit> Default => CreateUpperCaseLetter();
    
    /// <summary>
    /// Creates a formatter that formats as a single upper case letter, e.g. Spades = S, Diamonds = D.
    /// </summary>
    [Pure]
    public static IFormat<Suit> CreateUpperCaseLetter(bool caseInsensitiveParsing = true) => new EnumFormat<Suit>(caseInsensitiveParsing, "S", "H", "D", "C");
    
    /// <summary>
    /// Creates a formatter that formats as a single lower case letter, e.g. Spades = s, Diamonds = d.
    /// </summary>
    [Pure]
    public static IFormat<Suit> CreateLowerCaseLetter(bool caseInsensitiveParsing = true) => new EnumFormat<Suit>(caseInsensitiveParsing, "s", "h", "d", "c");
    
    /// <summary>
    /// Creates a formatter that formats as the Unicode black symbol, e.g. Spades = ♠, Diamonds = ♦.
    /// </summary>
    [Pure]
    public static IFormat<Suit> CreateBlackSymbol() => new EnumFormat<Suit>(false, "\u2660", "\u2665", "\u2666", "\u2663");
    
    /// <summary>
    /// Creates a formatter that formats as the Unicode white symbol, e.g. Spades = ♤, Diamonds = ♢.
    /// </summary>
    [Pure]
    public static IFormat<Suit> CreateWhiteSymbol() => new EnumFormat<Suit>(false, "\u2664", "\u2661", "\u2662", "\u2667");
    
    /// <summary>
    /// Creates a formatter that formats using the full English word in title case, E.g. Spades = Spades, Diamonds = Diamonds.
    /// </summary>
    [Pure]
    public static IFormat<Suit> CreateTitleCaseWord(bool caseInsensitiveParsing = true) => new EnumFormat<Suit>(caseInsensitiveParsing, "Spades", "Hearts", "Diamonds", "Clubs");
    
    /// <summary>
    /// Creates a formatter that formats using the full English word in lower case, E.g. Spades = spades, Diamonds = diamonds.
    /// </summary>
    [Pure]
    public static IFormat<Suit> CreateLowerCaseWord(bool caseInsensitiveParsing = true) => new EnumFormat<Suit>(caseInsensitiveParsing, "spades", "hearts", "diamonds", "clubs");
}