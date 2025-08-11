using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.CreateSalesItem
{
    public class AddSaleItemCommand : IRequest<AddSaleItemResult>
    {
        public Guid SaleId { get; set; }
        public string Product { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
