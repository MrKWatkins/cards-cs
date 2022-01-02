using FluentAssertions;
using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Text;

public sealed class CardFormatTests : FormatTestFixture
{
    private readonly Card[] cards = { new(Rank.Ace, Suit.Spades), new (Rank.Five, Suit.Hearts), new (Rank.Ten, Suit.Diamonds), new (Rank.King, Suit.Clubs) };

    [Test]
    public void Constructor_ThrowsIfNot52Cards() =>
        FluentActions.Invoking(() => new CardFormat(" ", true, "One", "Two")).Should().Throw<ArgumentException>();
    
    [TestCase(Rank.Ace, Suit.Spades, "AS")]
    [TestCase(Rank.Five, Suit.Hearts, "5H")]
    [TestCase(Rank.Ten, Suit.Diamonds, "10D")]
    [TestCase(Rank.King, Suit.Clubs, "KC")]
    public void UpperCaseLetters(Rank rank, Suit suit, string expected)
    {
        TestFormat(CardFormat.CreateUpperCaseLetters, expected, new Card(rank, suit));
        TestFormat(CardFormat.Default, true, expected, new Card(rank, suit));
    }
    
    [Test]
    public void UpperCaseLetters_Multiple()
    {
        TestFormat(CardFormat.CreateUpperCaseLetters, "AS 5H 10D KC", cards);
        TestFormat(CardFormat.Default, true, "AS 5H 10D KC", cards);
    }

    [TestCase(Rank.Ace, Suit.Spades, "as")]
    [TestCase(Rank.Five, Suit.Hearts, "5h")]
    [TestCase(Rank.Ten, Suit.Diamonds, "10d")]
    [TestCase(Rank.King, Suit.Clubs, "kc")]
    public void LowerCaseLetters(Rank rank, Suit suit, string expected) => TestFormat(CardFormat.CreateLowerCaseLetters, expected, new Card(rank, suit));
    
    [Test]
    public void LowerCaseLetters_Multiple() => TestFormat(CardFormat.CreateLowerCaseLetters, "as 5h 10d kc", cards);

    [TestCase(Rank.Ace, Suit.Spades, "Ace of Spades")]
    [TestCase(Rank.Five, Suit.Hearts, "Five of Hearts")]
    [TestCase(Rank.Ten, Suit.Diamonds, "Ten of Diamonds")]
    [TestCase(Rank.King, Suit.Clubs, "King of Clubs")]
    public void TitleCaseWords(Rank rank, Suit suit, string expected) => TestFormat(CardFormat.CreateTitleCaseWords, expected, new Card(rank, suit));

    [Test]
    public void TitleCaseWords_Multiple() => TestFormat(CardFormat.CreateTitleCaseWords, "Ace of Spades, Five of Hearts, Ten of Diamonds, King of Clubs", cards);

    [TestCase(Rank.Ace, Suit.Spades, "ace of spades")]
    [TestCase(Rank.Five, Suit.Hearts, "five of hearts")]
    [TestCase(Rank.Ten, Suit.Diamonds, "ten of diamonds")]
    [TestCase(Rank.King, Suit.Clubs, "king of clubs")]
    public void LowerCaseWords(Rank rank, Suit suit, string expected) => TestFormat(CardFormat.CreateLowerCaseWords, expected, new Card(rank, suit));
    
    [Test]
    public void LowerCaseWords_Multiple() => TestFormat(CardFormat.CreateLowerCaseWords, "ace of spades, five of hearts, ten of diamonds, king of clubs", cards);

    [TestCase(Rank.Ace, Suit.Spades, "\U0001F0A1")]
    [TestCase(Rank.Five, Suit.Hearts, "\U0001F0B5")]
    [TestCase(Rank.Ten, Suit.Diamonds, "\U0001F0CA")]
    [TestCase(Rank.King, Suit.Clubs, "\U0001F0DE")]
    public void Symbols(Rank rank, Suit suit, string expected) => TestFormat(CardFormat.CreateSymbols(), false, expected, new Card(rank, suit));
    
    [Test]
    public void Symbols_Multiple() => TestFormat(CardFormat.CreateSymbols(), false, "\U0001F0A1 \U0001F0B5 \U0001F0CA \U0001F0DE", cards);
}