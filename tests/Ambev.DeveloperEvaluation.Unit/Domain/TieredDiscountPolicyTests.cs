using Ambev.DeveloperEvaluation.Domain.Policies;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public class TieredDiscountPolicyTests
{
    private readonly TieredDiscountPolicy _policy = new();

    [Theory(DisplayName = "GetPercent returns correct discount for quantity")]
    [InlineData(1, 0.00)]
    [InlineData(3, 0.00)]
    [InlineData(4, 0.10)]
    [InlineData(5, 0.10)]
    [InlineData(9, 0.10)]
    [InlineData(10, 0.20)]
    [InlineData(15, 0.20)]
    [InlineData(20, 0.20)]
    [InlineData(21, 0.00)]
    [InlineData(0, 0.00)]
    public void GetPercent_ShouldReturnExpectedDiscount(int quantity, decimal expected)
    {
        // Act
        var result = _policy.GetPercent(quantity);
        // Assert
        result.Should().Be(expected);
    }
}
