using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace MrKWatkins.Cards.Text;

/// <summary>
/// A <see cref="IFormat{T}"/> for a bound set of indexable values.
/// </summary>
public abstract class IndexableFormat<T> : IFormat<T>
    where T : notnull
{
    private readonly IReadOnlyList<string> stringLookup;
    private readonly IReadOnlyDictionary<string, int> indexLookup;

    protected IndexableFormat(bool caseInsensitiveParsing, IReadOnlyList<string> values)
    {
        stringLookup = values;
        indexLookup = values
            .Select((v, i) => (Value: v, Index: i))
            .ToDictionary(t => t.Value, t => t.Index, caseInsensitiveParsing ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture);
    }

    [Pure]
    protected abstract int ToIndex(T value);
    
    [Pure]
    protected abstract T ToValue(int index);

    public string? Format(T? value) => value is not null ? stringLookup[ToIndex(value)] : null;

    public void AppendFormat(StringBuilder stringBuilder, T? value) => stringBuilder.Append(Format(value));

    public bool TryParse(string? value, [NotNullWhen(true)] out T? parsed)
    {
        if (value is not null && indexLookup.TryGetValue(value, out var index))
        {
            parsed = ToValue(index);
            return true;
        }

        parsed = default;
        return false;
    }
}