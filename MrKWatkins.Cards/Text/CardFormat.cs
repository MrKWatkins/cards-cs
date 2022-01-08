using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Text;

public sealed class CardFormat : IndexableFormat<Card>
{
    public static IFormat<Card> Default => CreateUpperCaseLetters();

    /// <summary>
    /// Formats using upper case letters and numbers, e.g. Ace of spades = AS, ten of hearts = 10H, queen of diamonds = QD.
    /// </summary>
    [Pure]
    public static IFormat<Card> CreateUpperCaseLetters(bool caseInsensitiveParsing = true) => new CardFormat(" ", caseInsensitiveParsing, RankFormat.CreateUpperCaseLetter(), SuitFormat.CreateUpperCaseLetter());
    
    /// <summary>
    /// Formats using upper case letters and numbers, e.g. Ace of spades = as, ten of hearts = 10h, queen of diamonds = qd.
    /// </summary>
    [Pure]
    public static IFormat<Card> CreateLowerCaseLetters(bool caseInsensitiveParsing = true) =>  new CardFormat(" ", caseInsensitiveParsing, RankFormat.CreateLowerCaseLetter(), SuitFormat.CreateLowerCaseLetter());
    
    /// <summary>
    /// Formats using title case words, e.g. Ace of spades = Ace of Spades, ten of hearts = Ten of Hearts, queen of diamonds = Queen of Diamonds.
    /// </summary>
    [Pure]
    public static IFormat<Card> CreateTitleCaseWords(bool caseInsensitiveParsing = true) =>  new CardFormat(", ", caseInsensitiveParsing, RankFormat.CreateTitleCaseWord(), SuitFormat.CreateTitleCaseWord(), " of ");
    
    /// <summary>
    /// Formats using lower case words, e.g. Ace of spades = ace of spades, ten of hearts = ten of hearts, queen of diamonds = queen of diamonds.
    /// </summary>
    [Pure]
    public static IFormat<Card> CreateLowerCaseWords(bool caseInsensitiveParsing = true) =>  new CardFormat(", ", caseInsensitiveParsing, RankFormat.CreateLowerCaseWord(), SuitFormat.CreateLowerCaseWord(), " of ");

    /// <summary>
    /// Formats using Unicode symbols, e.g. Ace of spades = 🂡, ten of hearts = 🂺, queen of diamonds = 🃍.
    /// </summary>
    [Pure]
    public static IFormat<Card> CreateSymbols() => new CardFormat(
        " ",
        false,
        "\U0001F0A1", "\U0001F0A2", "\U0001F0A3", "\U0001F0A4", "\U0001F0A5", "\U0001F0A6", "\U0001F0A7", "\U0001F0A8", "\U0001F0A9", "\U0001F0AA", "\U0001F0AB", "\U0001F0AD", "\U0001F0AE",
        "\U0001F0B1", "\U0001F0B2", "\U0001F0B3", "\U0001F0B4", "\U0001F0B5", "\U0001F0B6", "\U0001F0B7", "\U0001F0B8", "\U0001F0B9", "\U0001F0BA", "\U0001F0BB", "\U0001F0BD", "\U0001F0BE",
        "\U0001F0C1", "\U0001F0C2", "\U0001F0C3", "\U0001F0C4", "\U0001F0C5", "\U0001F0C6", "\U0001F0C7", "\U0001F0C8", "\U0001F0C9", "\U0001F0CA", "\U0001F0CB", "\U0001F0CD", "\U0001F0CE",
        "\U0001F0D1", "\U0001F0D2", "\U0001F0D3", "\U0001F0D4", "\U0001F0D5", "\U0001F0D6", "\U0001F0D7", "\U0001F0D8", "\U0001F0D9", "\U0001F0DA", "\U0001F0DB", "\U0001F0DD", "\U0001F0DE");
    
    public CardFormat(string multipleSeparator, bool caseInsensitiveParsing, IFormatter<Rank> rankFormat, IFormatter<Suit> suitFormat)
        : this(multipleSeparator, caseInsensitiveParsing, Card.FullDeck.Select(c => $"{rankFormat.Format(c.Rank)}{suitFormat.Format(c.Suit)}"))
    {
    }

    public CardFormat(string multipleSeparator, bool caseInsensitiveParsing, IFormatter<Rank> rankFormat, IFormatter<Suit> suitFormat, string separator)
        : this(multipleSeparator, caseInsensitiveParsing, Card.FullDeck.Select(c => $"{rankFormat.Format(c.Rank)}{separator}{suitFormat.Format(c.Suit)}"))
    {
    }

    public CardFormat(string multipleSeparator, bool caseInsensitiveParsing, [JetBrains.Annotations.InstantHandle] IEnumerable<string> values)
        : this(multipleSeparator, caseInsensitiveParsing, values.ToArray())
    {
    }
    
    public CardFormat(string multipleSeparator, bool caseInsensitiveParsing, params string[] values)
        : base(multipleSeparator, caseInsensitiveParsing, values)
    {
        if (values.Length != 52)
        {
            throw new ArgumentException("Value must have 52 entries.", nameof(values));
        }
    }

    protected override int ToIndex(Card value) => value.Index;

    protected override Card ToValue(int index) => Card.FromIndex(index);
}