using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Text;

public sealed class CardFormatterTests : FormatterTestFixture
{
    [TestCase(Rank.Ace, Suit.Spades, "AS")]
    [TestCase(Rank.Five, Suit.Hearts, "5H")]
    [TestCase(Rank.Ten, Suit.Diamonds, "10D")]
    [TestCase(Rank.King, Suit.Clubs, "KC")]
    public void UpperCaseLetters(Rank rank, Suit suit, string expected)
    {
        TestFormatter(CardFormatter.CreateUpperCaseLetters, new Card(rank, suit), expected);
        TestFormatter(CardFormatter.Default, new Card(rank, suit), expected);
    }

    [TestCase(Rank.Ace, Suit.Spades, "as")]
    [TestCase(Rank.Five, Suit.Hearts, "5h")]
    [TestCase(Rank.Ten, Suit.Diamonds, "10d")]
    [TestCase(Rank.King, Suit.Clubs, "kc")]
    public void LowerCaseLetters(Rank rank, Suit suit, string expected) => TestFormatter(CardFormatter.CreateLowerCaseLetters, new Card(rank, suit), expected);

    [TestCase(Rank.Ace, Suit.Spades, "Ace of Spades")]
    [TestCase(Rank.Five, Suit.Hearts, "Five of Hearts")]
    [TestCase(Rank.Ten, Suit.Diamonds, "Ten of Diamonds")]
    [TestCase(Rank.King, Suit.Clubs, "King of Clubs")]
    public void TitleCaseWords(Rank rank, Suit suit, string expected) => TestFormatter(CardFormatter.CreateTitleCaseWords, new Card(rank, suit), expected);

    [TestCase(Rank.Ace, Suit.Spades, "ace of spades")]
    [TestCase(Rank.Five, Suit.Hearts, "five of hearts")]
    [TestCase(Rank.Ten, Suit.Diamonds, "ten of diamonds")]
    [TestCase(Rank.King, Suit.Clubs, "king of clubs")]
    public void LowerCaseWords(Rank rank, Suit suit, string expected) => TestFormatter(CardFormatter.CreateLowerCaseWords, new Card(rank, suit), expected);
    
    [TestCase(Rank.Ace, Suit.Spades, "\U0001F0A1")]
    [TestCase(Rank.Five, Suit.Hearts, "\U0001F0B5")]
    [TestCase(Rank.Ten, Suit.Diamonds, "\U0001F0CA")]
    [TestCase(Rank.King, Suit.Clubs, "\U0001F0DE")]
    public void CreateSymbols(Rank rank, Suit suit, string expected) => TestFormatter(CardFormatter.CreateSymbols, new Card(rank, suit), expected);
}