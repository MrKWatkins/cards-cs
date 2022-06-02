using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace MrKWatkins.Cards.Testing;

public static class Operators<T>
{
    [MemberNotNullWhen(true, nameof(Equality))]
    public static bool HasEquality => Equality != null;
    
    public static Func<T?, T?, bool>? Equality { get; } = CreateCall("op_Equality");
    
    [MemberNotNullWhen(true, nameof(Inequality))]
    public static bool HasInequality => Inequality != null;

    public static Func<T?, T?, bool>? Inequality { get; } = CreateCall("op_Inequality");
    
    [MemberNotNullWhen(true, nameof(LessThan))]
    public static bool HasLessThan => LessThan != null;
    
    public static Func<T?, T?, bool>? LessThan { get; } = CreateCall("op_LessThan");
    
    [MemberNotNullWhen(true, nameof(LessThanOrEqual))]
    public static bool HasLessThanOrEqual => LessThanOrEqual != null;

    public static Func<T?, T?, bool>? LessThanOrEqual { get; } = CreateCall("op_LessThanOrEqual");
    
    [MemberNotNullWhen(true, nameof(GreaterThan))]
    public static bool HasGreaterThan => GreaterThan != null;

    public static Func<T?, T?, bool>? GreaterThan { get; } = CreateCall("op_GreaterThan");
    
    [MemberNotNullWhen(true, nameof(GreaterThanOrEqual))]
    public static bool HasGreaterThanOrEqual => GreaterThanOrEqual != null;

    public static Func<T?, T?, bool>? GreaterThanOrEqual { get; } = CreateCall("op_GreaterThanOrEqual");
    
    [Pure]
    private static Func<T?, T?, bool>? CreateCall(string name)
    {
        var @operator = GetOperator(name);

        return @operator != null
            ? (x, y) => CallOperator(@operator, x, y)
            : null;
    }
    
    [Pure]
    private static MethodInfo? GetOperator(string name) => typeof(T).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
    
    [Pure]
    private static bool CallOperator(MethodInfo @operator, T? x, T? y) => (bool)@operator.Invoke(null, new object?[] { x, y })!;
}