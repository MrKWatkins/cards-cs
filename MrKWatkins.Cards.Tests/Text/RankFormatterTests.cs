using MrKWatkins.Cards.Text;
using NUnit.Framework;

namespace MrKWatkins.Cards.Tests.Text;

public sealed class RankFormatterTests : FormatterTestFixture
{
    [TestCase(Rank.Ace, "A")]
    [TestCase(Rank.Two, "2")]
    [TestCase(Rank.Three, "3")]
    [TestCase(Rank.Four, "4")]
    [TestCase(Rank.Five, "5")]
    [TestCase(Rank.Six, "6")]
    [TestCase(Rank.Seven, "7")]
    [TestCase(Rank.Eight, "8")]
    [TestCase(Rank.Nine, "9")]
    [TestCase(Rank.Ten, "10")]
    [TestCase(Rank.Jack, "J")]
    [TestCase(Rank.Queen, "Q")]
    [TestCase(Rank.King, "K")]
    public void UpperCaseLetter(Rank rank, string expected)
    {
        TestFormatter(RankFormatter.CreateUpperCaseLetter, rank, expected);
        TestFormatter(RankFormatter.Default, rank, expected);
    }

    [TestCase(Rank.Ace, "a")]
    [TestCase(Rank.Two, "2")]
    [TestCase(Rank.Three, "3")]
    [TestCase(Rank.Four, "4")]
    [TestCase(Rank.Five, "5")]
    [TestCase(Rank.Six, "6")]
    [TestCase(Rank.Seven, "7")]
    [TestCase(Rank.Eight, "8")]
    [TestCase(Rank.Nine, "9")]
    [TestCase(Rank.Ten, "10")]
    [TestCase(Rank.Jack, "j")]
    [TestCase(Rank.Queen, "q")]
    [TestCase(Rank.King, "k")]
    public void LowerCaseLetter(Rank rank, string expected) => TestFormatter(RankFormatter.CreateLowerCaseLetter, rank, expected);

    [TestCase(Rank.Ace, "A")]
    [TestCase(Rank.Two, "2")]
    [TestCase(Rank.Three, "3")]
    [TestCase(Rank.Four, "4")]
    [TestCase(Rank.Five, "5")]
    [TestCase(Rank.Six, "6")]
    [TestCase(Rank.Seven, "7")]
    [TestCase(Rank.Eight, "8")]
    [TestCase(Rank.Nine, "9")]
    [TestCase(Rank.Ten, "T")]
    [TestCase(Rank.Jack, "J")]
    [TestCase(Rank.Queen, "Q")]
    [TestCase(Rank.King, "K")]
    public void UpperCaseLetterTFor10(Rank rank, string expected) => TestFormatter(RankFormatter.CreateUpperCaseLetterTFor10, rank, expected);
    
    [TestCase(Rank.Ace, "a")]
    [TestCase(Rank.Two, "2")]
    [TestCase(Rank.Three, "3")]
    [TestCase(Rank.Four, "4")]
    [TestCase(Rank.Five, "5")]
    [TestCase(Rank.Six, "6")]
    [TestCase(Rank.Seven, "7")]
    [TestCase(Rank.Eight, "8")]
    [TestCase(Rank.Nine, "9")]
    [TestCase(Rank.Ten, "t")]
    [TestCase(Rank.Jack, "j")]
    [TestCase(Rank.Queen, "q")]
    [TestCase(Rank.King, "k")]
    public void LowerCaseLetterTFor10(Rank rank, string expected) => TestFormatter(RankFormatter.CreateLowerCaseLetterTFor10, rank, expected);

    [TestCase(Rank.Ace, "Ace")]
    [TestCase(Rank.Two, "Two")]
    [TestCase(Rank.Three, "Three")]
    [TestCase(Rank.Four, "Four")]
    [TestCase(Rank.Five, "Five")]
    [TestCase(Rank.Six, "Six")]
    [TestCase(Rank.Seven, "Seven")]
    [TestCase(Rank.Eight, "Eight")]
    [TestCase(Rank.Nine, "Nine")]
    [TestCase(Rank.Ten, "Ten")]
    [TestCase(Rank.Jack, "Jack")]
    [TestCase(Rank.Queen, "Queen")]
    [TestCase(Rank.King, "King")]
    public void TitleCaseWord(Rank rank, string expected) => TestFormatter(RankFormatter.CreateTitleCaseWord, rank, expected);
    
    [TestCase(Rank.Ace, "ace")]
    [TestCase(Rank.Two, "two")]
    [TestCase(Rank.Three, "three")]
    [TestCase(Rank.Four, "four")]
    [TestCase(Rank.Five, "five")]
    [TestCase(Rank.Six, "six")]
    [TestCase(Rank.Seven, "seven")]
    [TestCase(Rank.Eight, "eight")]
    [TestCase(Rank.Nine, "nine")]
    [TestCase(Rank.Ten, "ten")]
    [TestCase(Rank.Jack, "jack")]
    [TestCase(Rank.Queen, "queen")]
    [TestCase(Rank.King, "king")]
    public void LowerCaseWord(Rank rank, string expected) => TestFormatter(RankFormatter.CreateLowerCaseWord, rank, expected);
}