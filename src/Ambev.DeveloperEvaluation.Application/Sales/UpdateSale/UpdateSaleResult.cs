namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
