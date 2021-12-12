using System.Text;

namespace MrKWatkins.Cards.Text;

public sealed class EnumFormatter<T> : IFormatter<T>
    where T : struct, Enum, IConvertible
{
    private readonly IReadOnlyList<string> values;
    
    public EnumFormatter(params string[] values)
    {
        // The checks on T could go in a static constructor but easier to handle and test instance level exceptions.
        if (Enum.GetUnderlyingType(typeof(T)) != typeof(int))
        {
            throw new ArgumentException("Enum type must have an underlying type of Int32.", nameof(T));
        }
        
        var enumValues = Enum.GetValues(typeof(T));
        var intValues = enumValues.OfType<object>().Select(e => (int)e);
        if (!intValues.SequenceEqual(Enumerable.Range(0, enumValues.Length)))
        {
            throw new ArgumentException("Enum type must have sequential values starting at 0.", nameof(T));
        }
        
        if (values.Length != enumValues.Length)
        {
            throw new ArgumentException($"Value must have {enumValues.Length} entries.", nameof(values));
        }
        
        this.values = values;
    }

    public string Format(T value) => values[value.ToInt32(null)];

    public void AppendFormat(StringBuilder stringBuilder, T value) => stringBuilder.Append(Format(value));
}