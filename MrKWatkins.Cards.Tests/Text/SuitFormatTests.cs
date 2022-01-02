using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Text;

public sealed class SuitFormatTests : FormatTestFixture
{
    [TestCase(Suit.Spades, "S")]
    [TestCase(Suit.Hearts, "H")]
    [TestCase(Suit.Diamonds, "D")]
    [TestCase(Suit.Clubs, "C")]
    public void UpperCaseLetter(Suit suit, string expected)
    {
        TestFormat(SuitFormat.CreateUpperCaseLetter, expected, suit);
        TestFormat(SuitFormat.Default, true, expected, suit);
    }

    [TestCase(Suit.Spades, "s")]
    [TestCase(Suit.Hearts, "h")]
    [TestCase(Suit.Diamonds, "d")]
    [TestCase(Suit.Clubs, "c")]
    public void LowerCaseLetter(Suit suit, string expected) => TestFormat(SuitFormat.CreateLowerCaseLetter, expected, suit);
    
    [TestCase(Suit.Spades, "\u2660")]
    [TestCase(Suit.Hearts, "\u2665")]
    [TestCase(Suit.Diamonds, "\u2666")]
    [TestCase(Suit.Clubs, "\u2663")]
    public void BlackSymbol(Suit suit, string expected) => TestFormat(SuitFormat.CreateBlackSymbol(), false, expected, suit);
    
    [TestCase(Suit.Spades, "\u2664")]
    [TestCase(Suit.Hearts, "\u2661")]
    [TestCase(Suit.Diamonds, "\u2662")]
    [TestCase(Suit.Clubs, "\u2667")]
    public void WhiteSymbol(Suit suit, string expected) => TestFormat(SuitFormat.CreateWhiteSymbol(), false, expected, suit);
    
    [TestCase(Suit.Spades, "Spades")]
    [TestCase(Suit.Hearts, "Hearts")]
    [TestCase(Suit.Diamonds, "Diamonds")]
    [TestCase(Suit.Clubs, "Clubs")]
    public void TitleCaseWord(Suit suit, string expected) => TestFormat(SuitFormat.CreateTitleCaseWord, expected, suit);
    
    [TestCase(Suit.Spades, "spades")]
    [TestCase(Suit.Hearts, "hearts")]
    [TestCase(Suit.Diamonds, "diamonds")]
    [TestCase(Suit.Clubs, "clubs")]
    public void LowerCaseWord(Suit suit, string expected) => TestFormat(SuitFormat.CreateLowerCaseWord, expected, suit);

}