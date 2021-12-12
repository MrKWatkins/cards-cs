using BenchmarkDotNet.Attributes;

namespace MrKWatkins.Cards.Benchmarks.Text;

/// <summary>
/// Benchmark to see if the extra effort in adding a char formatter helps with creating card strings.
/// </summary>
/// <remarks>
/// It would've been, but decided to make the CardFormatters use lookups instead.
/// </remarks>
[MemoryDiagnoser]
public class StringCreationBenchmark
{
    private readonly string string1;
    private readonly string string2;
    private readonly char char1;
    private readonly char char2;

    public StringCreationBenchmark()
    {
        // Use random strings to try and stop the JITter being too clever with optimisations.
        var random = new Random();
        string1 = random.Next() % 2 == 0 ? "A" : "B";
        string2 = random.Next() % 2 == 0 ? "A" : "B";
        char1 = random.Next() % 2 == 0 ? 'A' : 'B';
        char2 = random.Next() % 2 == 0 ? 'A' : 'B';
    }

    [Benchmark(Baseline = true)]
    public string FormatTwoStrings() => $"{string1}{string2}";
    
    [Benchmark]
    public string ConcatTwoStrings() => string1 + string2;
    
    [Benchmark]
    public string FormatTwoChars() => $"{char1}{char2}";

    [Benchmark]
    public string Stackalloc()
    {
        Span<char> chars = stackalloc char[2];
        chars[0] = char1;
        chars[1] = char2;
        return new string(chars);
    }
}