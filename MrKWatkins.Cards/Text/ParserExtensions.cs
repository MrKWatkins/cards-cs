using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Text;

public static class ParserExtensions
{
    [Pure]
    public static T? ParseOrDefault<T>(this IParser<T> parser, string? value) 
        where T : notnull => 
        parser.TryParse(value, out var index) ? index : default;

    [Pure]
    public static T ParseOrThrow<T>(this IParser<T> parser, string? value) 
        where T : notnull => 
        parser.TryParse(value, out var index) ? index : throw new FormatException($"Value \"{value}\" cannot be parsed to a {typeof(T).Name}.");
}