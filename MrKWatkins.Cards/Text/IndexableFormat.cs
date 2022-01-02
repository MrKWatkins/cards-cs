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
    private readonly string multipleSeparator;
    private readonly IReadOnlyList<string> stringLookup;
    private readonly IReadOnlyDictionary<string, int> indexLookup;

    protected IndexableFormat(string multipleSeparator, bool caseInsensitiveParsing, IReadOnlyList<string> values)
    {
        if (values.Any(v => v.Contains(multipleSeparator)))
        {
            throw new ArgumentException($"Value \"{multipleSeparator}\" is contained in one or more of {nameof(values)}.", nameof(multipleSeparator));
        }

        if (caseInsensitiveParsing && multipleSeparator.ToLowerInvariant() != multipleSeparator.ToUpperInvariant())
        {
            throw new ArgumentException($"Value \"{multipleSeparator}\" should be invariant with respect to case because {nameof(caseInsensitiveParsing)} is true.", nameof(multipleSeparator));
        }
        
        this.multipleSeparator = multipleSeparator;
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

    public string Format(IEnumerable<T> values)
    {
        var stringBuilder = new StringBuilder();
        AppendFormat(stringBuilder, values);
        return stringBuilder.ToString();
    }

    public void AppendFormat(StringBuilder stringBuilder, T? value) => stringBuilder.Append(Format(value));
    
    public void AppendFormat(StringBuilder stringBuilder, IEnumerable<T> values)
    {
        using var enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }

        stringBuilder.Append(Format(enumerator.Current));
        while (enumerator.MoveNext())
        {
            stringBuilder.Append(multipleSeparator);
            stringBuilder.Append(Format(enumerator.Current));
        }
    }

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

    public bool TryParseMultiple(string? value, [NotNullWhen(true)] out IReadOnlyList<T>? parsed)
    {
        if (value is null)
        {
            parsed = null;
            return false;
        }

        if (value.Length == 0)
        {
            parsed = Array.Empty<T>();
            return true;
        }

        var values = new List<T>();
        foreach (var singleString in value.Split(multipleSeparator))
        {
            if (!TryParse(singleString, out var singleValue))
            {
                parsed = null;
                return false;
            }
            values.Add(singleValue);
        }

        parsed = values;
        return true;
    }
}