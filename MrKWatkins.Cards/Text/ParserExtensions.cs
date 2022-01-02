using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Text;

public static class ParserExtensions
{
    [Pure]
    public static T? ParseOrDefault<T>(this IParser<T> parser, string? value) 
        where T : notnull => 
        parser.TryParse(value, out var result) ? result : default;
    
    [Pure]
    public static IReadOnlyList<T>? ParseMultipleOrNull<T>(this IParser<T> parser, string? value) 
        where T : notnull => 
        parser.TryParseMultiple(value, out var result) ? result : default;

    [Pure]
    public static T ParseOrThrow<T>(this IParser<T> parser, string? value) 
        where T : notnull => 
        parser.TryParse(value, out var result) ? result : throw new FormatException($"Value \"{value}\" cannot be parsed to a {typeof(T).Name}.");
    
    [Pure]
    public static IReadOnlyList<T> ParseMultipleOrThrow<T>(this IParser<T> parser, string? value) 
        where T : notnull => 
        parser.TryParseMultiple(value, out var result) ? result : throw new FormatException($"Value \"{value}\" cannot be parsed to a {typeof(T).Name}.");
}