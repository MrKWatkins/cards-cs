using System.Diagnostics.Contracts;
using FluentAssertions;
using NUnit.Framework;

namespace MrKWatkins.Cards.Testing;

public static class ComparableTests
{
    [Pure]
    public static IEnumerable<TestCaseData> CreateTestData<T>([JetBrains.Annotations.InstantHandle] Func<IEnumerable<T>> createOrderedInstances)
    {
        // Less than.
        return createOrderedInstances()
            .Zip(createOrderedInstances().Skip(1))
            .Select(x => new TestCaseData(x.First, x.Second, -1));
    }

    public static void AssertCompareTo<T>(T x, T y, int expectedSign)
        where T : IComparable<T>
    {
        x.CompareTo(y).Should().HaveTheSameSignAs(expectedSign);
        y.CompareTo(x).Should().HaveTheSameSignAs(-expectedSign);
        
        AssertOperators(x, y, expectedSign);
        AssertIComparable(x, y, expectedSign);
    }
    
    private static void AssertOperators<T>(T x, T? y, int expectedSign)
        where T : IComparable<T>
    {
        if (Operators<T>.HasLessThan)
        {
            Operators<T>.LessThan(x, y).Should().Be(expectedSign < 0);
            Operators<T>.LessThan(y, x).Should().Be(expectedSign >= 0);
        }
        
        if (Operators<T>.HasLessThanOrEqual)
        {
            Operators<T>.LessThanOrEqual(x, y).Should().Be(expectedSign <= 0);
            Operators<T>.LessThanOrEqual(y, x).Should().Be(expectedSign > 0);
        }
        
        if (Operators<T>.HasGreaterThan)
        {
            Operators<T>.GreaterThan(x, y).Should().Be(expectedSign > 0);
            Operators<T>.GreaterThan(y, x).Should().Be(expectedSign <= 0);
        }
        
        if (Operators<T>.HasGreaterThanOrEqual)
        {
            Operators<T>.GreaterThanOrEqual(x, y).Should().Be(expectedSign >= 0);
            Operators<T>.GreaterThanOrEqual(y, x).Should().Be(expectedSign < 0);
        }
    }
    
    private static void AssertIComparable<T>(T x, T? y, int expectedSign)
        where T : IComparable<T>
    {
        if (x is not IComparable comparableX)
        {
            return;
        }
        
        comparableX.CompareTo(y).Should().HaveTheSameSignAs(expectedSign);

        if (y is IComparable comparableY)
        {
            comparableY.CompareTo(x).Should().HaveTheSameSignAs(expectedSign);
        }
    }
}