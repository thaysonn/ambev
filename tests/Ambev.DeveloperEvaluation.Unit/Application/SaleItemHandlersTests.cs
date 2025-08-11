using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.SalesItem.CreateSalesItem;
using Ambev.DeveloperEvaluation.Application.SalesItem.RemoveSalesItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Policies;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class AddSaleItemCommandHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly IDiscountPolicy _discountPolicy;
    private readonly AddSaleItemCommandHandler _handler;

    public AddSaleItemCommandHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _discountPolicy = Substitute.For<IDiscountPolicy>();
        _handler = new AddSaleItemCommandHandler(_repo, _discountPolicy);
    }

    [Fact(DisplayName = "Given valid item When adding to sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem>() };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _repo.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _discountPolicy.GetPercent(Arg.Any<int>()).Returns(0m);
        var command = new AddSaleItemCommand
        {
            SaleId = sale.Id,
            Product = "Beer",
            Quantity = 2,
            UnitPrice = 10
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sale.Items.Should().ContainSingle(i => i.Product == command.Product);
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale not found When adding item Then returns error response")]
    public async Task Handle_SaleNotFound_ReturnsError()
    {
        // Arrange
        var command = new AddSaleItemCommand { SaleId = Guid.NewGuid(), Product = "Beer", Quantity = 2, UnitPrice = 10 };
        _repo.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Sale not found"));
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given quantity > 20 When adding item Then returns error response")]
    public async Task Handle_QuantityGreaterThan20_ReturnsError()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem>() };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        var command = new AddSaleItemCommand { SaleId = sale.Id, Product = "Beer", Quantity = 21, UnitPrice = 10 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Cannot sell more than 20 units"));
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public class RemoveSaleItemCommandHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly RemoveSaleItemCommandHandler _handler;

    public RemoveSaleItemCommandHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _handler = new RemoveSaleItemCommandHandler(_repo);
    }

    [Fact(DisplayName = "Given valid sale and item When removing item Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var item = new SaleItem { Id = Guid.NewGuid(), Product = "Beer", Quantity = 2, UnitPrice = 10, Total = 20 };
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem> { item }, TotalAmount = 20 };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _repo.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var command = new RemoveSaleItemCommand { SaleId = sale.Id, ItemId = item.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sale.Items.Should().BeEmpty();
        sale.TotalAmount.Should().Be(0);
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale not found When removing item Then returns error response")]
    public async Task Handle_SaleNotFound_ReturnsError()
    {
        // Arrange
        var command = new RemoveSaleItemCommand { SaleId = Guid.NewGuid(), ItemId = Guid.NewGuid() };
        _repo.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Sale not found"));
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given item not found When removing item Then returns error response")]
    public async Task Handle_ItemNotFound_ReturnsError()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Items = new List<SaleItem>() };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        var command = new RemoveSaleItemCommand { SaleId = sale.Id, ItemId = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Item not found"));
        await _repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
