using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SalesTests;

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
