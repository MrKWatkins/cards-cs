using System.Text;
using FluentAssertions;
using MrKWatkins.Cards.Text;

namespace MrKWatkins.Cards.Tests.Text;

public abstract class FormatterTestFixture
{
    protected static void TestFormatter<T>(Func<IFormatter<T>> formatterConstructor, T value, string expected) =>
        TestFormatter(formatterConstructor(), value, expected);

    protected static void TestFormatter<T>(IFormatter<T> formatter, T value, string expected)
    {
        formatter.Format(value).Should().Be(expected);

        var stringBuilder = new StringBuilder();
        formatter.AppendFormat(stringBuilder, value);
        stringBuilder.ToString().Should().Be(expected);
    }
}