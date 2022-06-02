using System.Diagnostics.Contracts;
using FluentAssertions;
using NUnit.Framework;

namespace MrKWatkins.Cards.Testing;

public static class EqualityTests
{
    [Pure]
    public static IEnumerable<TestCaseData> CreateTestData<T>([JetBrains.Annotations.InstantHandle] Func<IEnumerable<T>> createDifferentInstances) =>
        CreateTestData(createDifferentInstances, createDifferentInstances);
    
    [Pure]
    public static IEnumerable<TestCaseData> CreateTestData<T>([JetBrains.Annotations.InstantHandle] Func<IEnumerable<T>> createLeftInstances, [JetBrains.Annotations.InstantHandle] Func<IEnumerable<T>> createRightInstances) =>
        createLeftInstances().SelectMany((x, f) => createRightInstances().Select((y, g) => new TestCaseData(x, y, f == g)));
    
    public static void AssertEqual<T>(T x, T y, bool expectedEqual)
        where T : IEquatable<T>
    {
        if (expectedEqual)
        {
            AssertEqual(x, y);
        }
        else
        {
            AssertNotEqual(x, y);
        }
    }
    
    public static void AssertEqual<T>(T x, T y)
        where T : IEquatable<T>
    {
        x.Equals(x).Should().BeTrue();
        y.Equals(y).Should().BeTrue();
        
        x.Equals(y).Should().BeTrue();
        y.Equals(x).Should().BeTrue();
        
        x.Equals((object) y).Should().BeTrue();
        y.Equals((object) x).Should().BeTrue();
        
        x.GetHashCode().Should().Be(y.GetHashCode());
        y.GetHashCode().Should().Be(x.GetHashCode());

        AssertEqualityOperators(x, y, true);
        AssertIComparable(x, y, true);
        AssertGenericIComparable(x, y, true);
    }
    
    public static void AssertNotEqual<T>(T x, T y)
        where T : IEquatable<T>
    {
        x.Equals(y).Should().BeFalse();
        y.Equals(x).Should().BeFalse();
        
        x.Equals((object) y).Should().BeFalse();
        y.Equals((object) x).Should().BeFalse();

        AssertEqualityOperators(x, y, false);
        AssertIComparable(x, y, false);
        AssertGenericIComparable(x, y, false);
    }
    
    public static void AssertNotEqualToNull<T>(T x)
        where T : IEquatable<T>
    {
        x.Equals(null).Should().BeFalse();
        if (default(T) != null)
        {
            return;
        }
        
        x!.Equals(default).Should().BeFalse();

        AssertEqualityOperators(x, default, false);
        AssertIComparable(x, default, false);
        AssertGenericIComparable(x, default, false);
    }

    private static void AssertEqualityOperators<T>(T x, T? y, bool expectedEqual)
        where T : IEquatable<T>
    {
        if (Operators<T>.HasEquality)
        {
            Operators<T>.Equality(x, y).Should().Be(expectedEqual);
            Operators<T>.Equality(y, x).Should().Be(expectedEqual);
        }
        
        if (Operators<T>.HasInequality)
        {
            Operators<T>.Inequality(x, y).Should().Be(!expectedEqual);
            Operators<T>.Inequality(y, x).Should().Be(!expectedEqual);
        }

        if (!expectedEqual)
        {
            return;
        }
        
        if (Operators<T>.HasLessThanOrEqual)
        {
            Operators<T>.LessThanOrEqual(x, y).Should().BeTrue();
            Operators<T>.LessThanOrEqual(y, x).Should().BeTrue();
        }
        
        if (Operators<T>.HasGreaterThanOrEqual)
        {
            Operators<T>.GreaterThanOrEqual(x, y).Should().BeTrue();
            Operators<T>.GreaterThanOrEqual(y, x).Should().BeTrue();
        }
    }
    
    private static void AssertIComparable<T>(T x, T? y, bool expectedEqual)
        where T : IEquatable<T>
    {
        if (x is not IComparable comparableX)
        {
            return;
        }
        
        (comparableX.CompareTo(y) == 0).Should().Be(expectedEqual);

        if (y is IComparable comparableY)
        {
            (comparableY.CompareTo(x) == 0).Should().Be(expectedEqual);
        }
    }
    
    private static void AssertGenericIComparable<T>(T x, T? y, bool expectedEqual)
        where T : IEquatable<T>
    {
        if (x is not IComparable<T> comparableX)
        {
            return;
        }
        
        (comparableX.CompareTo(y) == 0).Should().Be(expectedEqual);

        if (y is IComparable<T> comparableY)
        {
            (comparableY.CompareTo(x) == 0).Should().Be(expectedEqual);
        }
    }
}