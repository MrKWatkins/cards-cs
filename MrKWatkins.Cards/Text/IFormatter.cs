using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace MrKWatkins.Cards.Text;

public interface IFormatter<in T>
{
    [Pure]
    [return: NotNullIfNotNull("value")]
    string? Format(T? value);
    
    void AppendFormat(StringBuilder stringBuilder, T? value);
}
