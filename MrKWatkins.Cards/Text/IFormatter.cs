using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace MrKWatkins.Cards.Text;

public interface IFormatter<in T>
    where T : notnull
{
    [Pure]
    [return: NotNullIfNotNull("value")]
    string? Format(T? value);
    
    [Pure]
    string Format([JetBrains.Annotations.InstantHandle] IEnumerable<T> values);
    
    void AppendFormat(StringBuilder stringBuilder, T? value);
    
    void AppendFormat(StringBuilder stringBuilder, [JetBrains.Annotations.InstantHandle] IEnumerable<T> values);
}
