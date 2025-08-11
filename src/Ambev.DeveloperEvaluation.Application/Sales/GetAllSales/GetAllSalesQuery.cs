using MediatR;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.Sales.Shared;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

public class GetAllSalesQuery : IRequest<List<SaleDto>>
{
}
