namespace MrKWatkins.Cards.Text;

public static class RankFormatter
{
    public static IFormatter<Rank> Default => CreateUpperCaseLetter();
    
    /// <summary>
    /// Creates a formatter that formats ace and picture cards with a single upper case letter, uses numbers for the other cards, e.g. Ace = A, Five = 5, Ten = 10, King = K.
    /// </summary>
    public static IFormatter<Rank> CreateUpperCaseLetter() => new EnumFormatter<Rank>("A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K");
    
    /// <summary>
    /// Creates a formatter that formats ace and picture cards with a single lower case letter, uses numbers for the other cards, e.g. Ace = a, Five = 5, Ten = 10, King = k.
    /// </summary>
    public static IFormatter<Rank> CreateLowerCaseLetter() => new EnumFormatter<Rank>("a", "2", "3", "4", "5", "6", "7", "8", "9", "10", "j", "q", "k");
    
    /// <summary>
    /// Creates a formatter that formats ace and picture cards with a single upper case letter, uses "T" for 10 and uses numbers for the other cards, e.g. Ace = A, Five = 5, Ten = T, King = K.
    /// </summary>
    public static IFormatter<Rank> CreateUpperCaseLetterTFor10() => new EnumFormatter<Rank>("A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K");
    
    /// <summary>
    /// Creates a formatter that formats ace and picture cards with a single lower case letter, uses "t" for 10 and uses numbers for the other cards, e.g. Ace = a, Five = 5, Ten = t, King = k.
    /// </summary>
    public static IFormatter<Rank> CreateLowerCaseLetterTFor10() => new EnumFormatter<Rank>("a", "2", "3", "4", "5", "6", "7", "8", "9", "t", "j", "q", "k");
    
    /// <summary>
    /// Creates a formatter that formats using the full English word in title case, E.g. Ace = Ace, Five = Five, Ten = Ten, King = King.
    /// </summary>
    public static IFormatter<Rank> CreateTitleCaseWord() => new EnumFormatter<Rank>("Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King");
    
    /// <summary>
    /// Creates a formatter that formats using the full English word in lower case, E.g. Ace = ace, Five = five, Ten = ten, King = king.
    /// </summary>
    public static IFormatter<Rank> CreateLowerCaseWord() => new EnumFormatter<Rank>("ace", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "jack", "queen", "king");
}