using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Policies;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SalesTests;

public class CreateSaleCommandHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly IDiscountPolicy _discountPolicy;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _discountPolicy = Substitute.For<IDiscountPolicy>();
        _handler = new CreateSaleCommandHandler(_repo, _discountPolicy);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "S001",
            Date = DateTime.UtcNow,
            Customer = "Customer A",
            Branch = "Branch X",
            Items = new List<CreateSaleItemCommand>
            {
                new() { Product = "Beer", Quantity = 2, UnitPrice = 10 }
            }
        };
        _discountPolicy.GetPercent(Arg.Any<int>()).Returns(0m);
        _repo.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _repo.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.SaleNumber.Should().Be(command.SaleNumber);
        result.SaleId.Should().NotBeEmpty();
        await _repo.Received(1).AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given item with quantity > 20 When creating sale Then returns error response")]
    public async Task Handle_QuantityGreaterThan20_ReturnsError()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "S002",
            Date = DateTime.UtcNow,
            Customer = "Customer B",
            Branch = "Branch Y",
            Items = new List<CreateSaleItemCommand>
            {
                new() { Product = "Beer", Quantity = 21, UnitPrice = 10 }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Cannot sell more than 20 units"));
        await _repo.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
