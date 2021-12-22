using System.Diagnostics.Contracts;
using System.Reflection;
using FluentAssertions;

namespace MrKWatkins.Cards.Tests;

public static class EqualityTests
{
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
        var equality = GetOperator<T>("op_Equality");
        if (equality != null)
        {
            CallOperator(equality, x, y).Should().Be(expectedEqual);
            CallOperator(equality, y, x).Should().Be(expectedEqual);
        }
        
        var inequality = GetOperator<T>("op_Inequality");
        if (inequality != null)
        {
            CallOperator(inequality, x, y).Should().Be(!expectedEqual);
            CallOperator(inequality, y, x).Should().Be(!expectedEqual);
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

    [Pure]
    private static MethodInfo? GetOperator<T>(string name) => typeof(T).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
    
    [Pure]
    private static bool CallOperator<T>(MethodInfo @operator, T? x, T? y) => (bool)@operator.Invoke(null, new object?[] { x, y })!;
}