namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleResult
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
