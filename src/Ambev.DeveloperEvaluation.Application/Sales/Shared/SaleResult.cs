namespace Ambev.DeveloperEvaluation.Application.Sales.Shared;

public class SaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool Cancelled { get; set; }
    public List<SaleItemResult> Items { get; set; } = new();
}
