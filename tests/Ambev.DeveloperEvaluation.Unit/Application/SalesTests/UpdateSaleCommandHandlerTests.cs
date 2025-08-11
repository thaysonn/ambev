using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SalesTests;

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
