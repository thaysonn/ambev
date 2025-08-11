using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.Shared;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

public class GetAllSalesQuery : IRequest<List<SaleResult>>
{
}
