namespace MrKWatkins.Cards.Text;

public interface IFormat<T> : IFormatter<T>, IParser<T>
    where T : notnull
{
}