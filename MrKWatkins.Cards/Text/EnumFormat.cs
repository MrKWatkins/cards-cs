namespace MrKWatkins.Cards.Text;

public sealed class EnumFormat<T> : IndexableFormat<T>
    where T : struct, Enum, IConvertible
{
    public EnumFormat(bool caseInsensitiveParsing, params string[] values)
        : this(" ", caseInsensitiveParsing, values)
    {
    }

    public EnumFormat(string multipleSeparator, bool caseInsensitiveParsing, params string[] values)
        : base(multipleSeparator, caseInsensitiveParsing, values)
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
    }

    protected override int ToIndex(T value) => value.ToInt32(null);

    protected override T ToValue(int index) => (T)(object)index;
}