using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.RemoveSalesItem
{
    public class RemoveSaleItemCommand : IRequest<RemoveSaleItemResult>
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
    }
}
