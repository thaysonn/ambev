using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleCommand : IRequest<DeleteSaleResult>
{
    public Guid Id { get; set; }
}

public class DeleteSaleResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
