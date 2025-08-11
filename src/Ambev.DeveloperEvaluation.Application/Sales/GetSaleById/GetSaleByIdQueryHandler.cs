
using Ambev.DeveloperEvaluation.Application.Sales.Shared;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleResult?>
{
    private readonly ISaleRepository _repo;
    public GetSaleByIdQueryHandler(ISaleRepository repo) { _repo = repo; }

    public async Task<SaleResult?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null) return null;
        return new SaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Date = sale.Date,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            Cancelled = sale.Cancelled,
            Items = sale.Items.Select(i => new SaleItemResult
            {
                Product = i.Product,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                Total = i.Total,
                Cancelled = i.Cancelled
            }).ToList()
        };
    }
}
