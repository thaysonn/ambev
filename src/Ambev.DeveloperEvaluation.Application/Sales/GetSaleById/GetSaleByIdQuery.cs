using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.Shared;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

public class GetSaleByIdQuery : IRequest<SaleResult?>
{
    public Guid Id { get; set; }
}
