using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Numeric;

namespace MrKWatkins.Cards.Testing;

public static class FluentAssertionExtensions
{
    public static AndConstraint<NumericAssertions<int>> HaveTheSameSignAs(this NumericAssertions<int> assertions, int expected, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .ForCondition(assertions.Subject.HasValue && Math.Sign(assertions.Subject.Value) == Math.Sign(expected))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:value} to be have the same sign as {0}{reason}, but found {1}", expected, assertions.Subject);

        return new AndConstraint<NumericAssertions<int>>(assertions);
    }
}