using System.Diagnostics.Contracts;
using System.Text;
using FluentAssertions;
using MrKWatkins.Cards.Text;

namespace MrKWatkins.Cards.Tests.Text;

public abstract class FormatTestFixture
{
    protected static void TestFormat<T>(Func<bool, IFormat<T>> formatConstructor, T value, string stringValue) 
        where T : notnull
    {
        TestFormat(formatConstructor(true), true, value, stringValue);
        TestFormat(formatConstructor(false), false, value, stringValue);
    }

    protected static void TestFormat<T>(IFormat<T> format, bool caseInsensitive, T value, string stringValue)
        where T : notnull
    {
        TestFormatter(format, value, stringValue);
        TestParser(format, caseInsensitive, value, stringValue);
    }

    private static void TestParser<T>(IParser<T> parser, bool caseInsensitive, T value, string stringValue)
        where T : notnull
    {
        parser.TryParse(stringValue, out var parsed).Should().BeTrue();
        parsed.Should().Be(value);
        parser.ParseOrDefault(stringValue).Should().Be(value);
        parser.ParseOrThrow(stringValue).Should().Be(value);

        if (!TryChangeCase(stringValue, out var differentCase))
        {
            return;
        }

        if (caseInsensitive)
        {
            parser.TryParse(differentCase, out parsed).Should().BeTrue();
            parsed.Should().Be(value);
            parser.ParseOrDefault(differentCase).Should().Be(value);
            parser.ParseOrThrow(differentCase).Should().Be(value);
        }
        else
        {
            parser.TryParse(differentCase, out parsed).Should().BeFalse();
            parser.ParseOrDefault(differentCase).Should().Be(default(T));
            parser.Invoking(p => p.ParseOrThrow(differentCase)).Should().Throw<FormatException>();
        }
    }

    [Pure]
    private static bool TryChangeCase(string value, out string differentCase)
    {
        differentCase = value.ToUpperInvariant();
        if (differentCase != value)
        {
            differentCase = value.ToLowerInvariant();
        }

        return differentCase != value;
    }

    private static void TestFormatter<T>(IFormatter<T> formatter, T value, string stringValue)
        where T : notnull
    {
        formatter.Format(value).Should().Be(stringValue);

        var stringBuilder = new StringBuilder();
        formatter.AppendFormat(stringBuilder, value);
        stringBuilder.ToString().Should().Be(stringValue);
    }
}