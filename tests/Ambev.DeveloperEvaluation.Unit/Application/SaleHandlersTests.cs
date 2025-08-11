using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Policies;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

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

public class UpdateSaleCommandHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly UpdateSaleCommandHandler _handler;

    public UpdateSaleCommandHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _handler = new UpdateSaleCommandHandler(_repo);
    }

    [Fact(DisplayName = "Given valid update data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid() };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _repo.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _repo.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            SaleNumber = "S003",
            Date = DateTime.UtcNow,
            Customer = "Customer C",
            Branch = "Branch Z"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        await _repo.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale When updating sale Then returns error response")]
    public async Task Handle_NonExistingSale_ReturnsError()
    {
        // Arrange
        var command = new UpdateSaleCommand { Id = Guid.NewGuid() };
        _repo.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Sale not found"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}

public class CancelSaleCommandHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly CancelSaleCommandHandler _handler;

    public CancelSaleCommandHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _handler = new CancelSaleCommandHandler(_repo);
    }

    [Fact(DisplayName = "Given valid sale id When cancelling sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Cancelled = false, Items = new List<SaleItem> { new SaleItem() } };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _repo.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _repo.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var command = new CancelSaleCommand { Id = sale.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sale.Cancelled.Should().BeTrue();
        sale.Items.All(i => i.Cancelled).Should().BeTrue();
        await _repo.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing sale When cancelling sale Then returns error response")]
    public async Task Handle_NonExistingSale_ReturnsError()
    {
        // Arrange
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };
        _repo.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Sale not found"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given already cancelled sale When cancelling sale Then returns error response")]
    public async Task Handle_AlreadyCancelledSale_ReturnsError()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Cancelled = true };
        _repo.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        var command = new CancelSaleCommand { Id = sale.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Sale already cancelled"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
