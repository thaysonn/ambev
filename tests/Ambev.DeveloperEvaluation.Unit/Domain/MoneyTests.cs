using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public class MoneyTests
{
    [Theory(DisplayName = "Round2 should round to two decimal places using AwayFromZero")]
    [InlineData(10.123, 10.12)]
    [InlineData(10.125, 10.13)]
    [InlineData(10.126, 10.13)]
    [InlineData(10.124, 10.12)]
    [InlineData(0.005, 0.01)]
    [InlineData(-1.235, -1.24)]
    [InlineData(-1.234, -1.23)]
    public void Round2_ShouldRoundCorrectly(decimal value, decimal expected)
    {
        // Act
        var result = Money.Round2(value);
        // Assert
        result.Should().Be(expected);
    }
}
