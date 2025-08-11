using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.UpdateSalesItem
{
    public class UpdateSaleItemCommand : IRequest<UpdateSaleItemResult>
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }  
    }
}
