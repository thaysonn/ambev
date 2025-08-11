namespace Ambev.DeveloperEvaluation.Application.SalesItem.CreateSalesItem
{
    public class AddSaleItemResult
    {
        public bool Success { get; set; }
        public Guid ItemId { get; set; }
        public string[]? Errors { get; set; }
    }
}
