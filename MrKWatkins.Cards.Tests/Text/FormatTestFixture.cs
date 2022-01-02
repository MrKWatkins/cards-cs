using System.Diagnostics.Contracts;
using System.Text;
using FluentAssertions;
using MrKWatkins.Cards.Text;

namespace MrKWatkins.Cards.Tests.Text;

public abstract class FormatTestFixture
{
    protected static void TestFormat<T>(Func<bool, IFormat<T>> formatConstructor, string stringValue, params T[] values) 
        where T : notnull
    {
        TestFormat(formatConstructor(true), true, stringValue, values);
        TestFormat(formatConstructor(false), false, stringValue, values);
    }

    protected static void TestFormat<T>(IFormat<T> format, bool caseInsensitive, string stringValue, params T[] values)
        where T : notnull
    {
        TestFormatterEmpty(format);
        TestParseNull(format);
        TestParseEmpty(format);
        
        if (values.Length == 1)
        {
            TestFormatter(format, stringValue, values[0]);
            TestParser(format, caseInsensitive, stringValue, values[0]);
        }
        
        TestFormatter(format, stringValue, values);
        TestParser(format, caseInsensitive, stringValue, values);
    }

    private static void TestFormatterEmpty<T>(IFormatter<T> formatter)
        where T : notnull
    {
        formatter.Format(Array.Empty<T>()).Should().Be("");

        var stringBuilder = new StringBuilder();
        formatter.AppendFormat(stringBuilder, Array.Empty<T>());
        stringBuilder.ToString().Should().Be("");
    }
    
    private static void TestFormatter<T>(IFormatter<T> formatter, string stringValue, IReadOnlyList<T> values)
        where T : notnull
    {
        formatter.Format(values).Should().Be(stringValue);

        var stringBuilder = new StringBuilder();
        formatter.AppendFormat(stringBuilder, values);
        stringBuilder.ToString().Should().Be(stringValue);
    }
    
    private static void TestFormatter<T>(IFormatter<T> formatter, string stringValue, T value)
        where T : notnull
    {
        formatter.Format(value).Should().Be(stringValue);

        var stringBuilder = new StringBuilder();
        formatter.AppendFormat(stringBuilder, value);
        stringBuilder.ToString().Should().Be(stringValue);
    }

    private static void TestParseNull<T>(IParser<T> parser)
        where T : notnull
    {
        parser.TryParse(null, out _).Should().BeFalse();
        parser.ParseOrDefault(null).Should().Be(default(T));
        parser.Invoking(p => p.ParseOrThrow(null)).Should().Throw<FormatException>();
        
        parser.TryParseMultiple(null, out _).Should().BeFalse();
        parser.ParseMultipleOrNull(null).Should().BeNull();
        parser.Invoking(p => p.ParseMultipleOrThrow(null)).Should().Throw<FormatException>();
    }
    
    private static void TestParseEmpty<T>(IParser<T> parser)
        where T : notnull
    {
        parser.TryParse("", out _).Should().BeFalse();
        parser.ParseOrDefault("").Should().Be(default(T));
        parser.Invoking(p => p.ParseOrThrow("")).Should().Throw<FormatException>();
        
        parser.TryParseMultiple("", out var parsed).Should().BeTrue();
        parsed.Should().BeEmpty();
        parser.ParseMultipleOrNull("").Should().BeEmpty();
        parser.ParseMultipleOrThrow("").Should().BeEmpty();
    }

    private static void TestParser<T>(IParser<T> parser, bool caseInsensitive, string stringValue, IReadOnlyList<T> values)
        where T : notnull
    {
        parser.TryParseMultiple(stringValue, out var parsed).Should().BeTrue();
        parsed.Should().BeEquivalentTo(values);
        parser.ParseMultipleOrNull(stringValue).Should().BeEquivalentTo(values);
        parser.ParseMultipleOrThrow(stringValue).Should().BeEquivalentTo(values);

        if (!TryChangeCase(stringValue, out var differentCase))
        {
            return;
        }

        if (caseInsensitive)
        {
            parser.TryParseMultiple(differentCase, out parsed).Should().BeTrue();
            parsed.Should().BeEquivalentTo(values);
            parser.ParseMultipleOrNull(differentCase).Should().BeEquivalentTo(values);
            parser.ParseMultipleOrThrow(differentCase).Should().BeEquivalentTo(values);
        }
        else
        {
            parser.TryParseMultiple(differentCase, out parsed).Should().BeFalse();
            parser.ParseMultipleOrNull(differentCase).Should().BeNull();
            parser.Invoking(p => p.ParseMultipleOrThrow(differentCase)).Should().Throw<FormatException>();
        }
    }
    
    private static void TestParser<T>(IParser<T> parser, bool caseInsensitive, string stringValue, T value)
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
}