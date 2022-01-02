using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace MrKWatkins.Cards.Text;

public interface IParser<T>
    where T : notnull
{
    [Pure]
    bool TryParse(string? value, [NotNullWhen(true)] out T? parsed);
    
    [Pure]
    bool TryParseMultiple(string? value, [NotNullWhen(true)] out IReadOnlyList<T>? parsed);
}