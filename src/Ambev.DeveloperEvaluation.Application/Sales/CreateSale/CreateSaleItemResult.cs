namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItemResult
{
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}