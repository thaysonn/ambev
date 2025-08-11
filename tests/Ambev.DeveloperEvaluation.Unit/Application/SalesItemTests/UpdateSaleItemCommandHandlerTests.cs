using Ambev.DeveloperEvaluation.Application.SalesItem.UpdateSalesItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Policies;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SalesItemTests;

public class UpdateSaleItemCommandHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly IDiscountPolicy _discountPolicy;
    private readonly UpdateSaleItemCommandHandler _handler;

    public UpdateSaleItemCommandHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _discountPolicy = Substitute.For<IDiscountPolicy>();
        _handler = new UpdateSaleItemCommandHandler(_repo, _discountPolicy);
    }

    [Fact(DisplayName = "Given valid item When updating sale item Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var item = new SaleItem { Id = Guid.NewGuid(), Product = "Beer", Quantity = 2, UnitPrice = 10, Total = 20 };
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem> { item }, TotalAmount = 20 };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _repo.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _repo.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _discountPolicy.GetPercent(Arg.Any<int>()).Returns(0m);
        var command = new UpdateSaleItemCommand
        {
            SaleId = sale.Id,
            ItemId = item.Id,
            Product = item.Product,
            Quantity = 5,
            UnitPrice = 15
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        item.Quantity.Should().Be(command.Quantity);
        item.UnitPrice.Should().Be(command.UnitPrice);
        await _repo.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale not found When updating item Then returns error response")]
    public async Task Handle_SaleNotFound_ReturnsError()
    {
        // Arrange
        var command = new UpdateSaleItemCommand { SaleId = Guid.NewGuid(), ItemId = Guid.NewGuid(), Product = "Beer", Quantity = 2, UnitPrice = 10 };
        _repo.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Sale not found"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given item not found When updating item Then returns error response")]
    public async Task Handle_ItemNotFound_ReturnsError()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem>() };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        var command = new UpdateSaleItemCommand { SaleId = sale.Id, ItemId = Guid.NewGuid(), Product = "Beer", Quantity = 2, UnitPrice = 10 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Item not found"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given quantity > 20 When updating item Then returns error response")]
    public async Task Handle_QuantityGreaterThan20_ReturnsError()
    {
        // Arrange
        var item = new SaleItem { Id = Guid.NewGuid(), Product = "Beer", Quantity = 2, UnitPrice = 10, Total = 20 };
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem> { item }, TotalAmount = 20 };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        var command = new UpdateSaleItemCommand { SaleId = sale.Id, ItemId = item.Id, Product = item.Product, Quantity = 21, UnitPrice = 10 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Cannot sell more than 20 units"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
