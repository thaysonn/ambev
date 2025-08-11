using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    public Guid Id { get; set; }
}

public class CancelSaleResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
