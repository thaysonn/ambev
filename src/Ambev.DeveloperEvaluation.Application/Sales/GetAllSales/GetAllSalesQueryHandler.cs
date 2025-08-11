
using Ambev.DeveloperEvaluation.Application.Sales.Shared;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, List<SaleDto>>
{
    private readonly ISaleRepository _repo;
    public GetAllSalesQueryHandler(ISaleRepository repo) { _repo = repo; }

    public async Task<List<SaleDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _repo.GetAllAsync(cancellationToken);
        return sales.Select(sale => new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Date = sale.Date,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            Cancelled = sale.Cancelled,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Product = i.Product,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                Total = i.Total,
                Cancelled = i.Cancelled
            }).ToList()
        }).ToList();
    }
}
