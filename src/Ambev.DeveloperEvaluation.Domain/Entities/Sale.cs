using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Policies;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool Cancelled { get; set; }
    public List<SaleItem> Items { get; set; } = new();

    // Novo método de fábrica
    public static Sale Create(string saleNumber, DateTime date, string customer, string branch, IEnumerable<(string Product, int Quantity, decimal UnitPrice)> items, IDiscountPolicy discountPolicy)
    {
        var saleItems = items.Select(i => SaleItem.Create(i.Product, i.Quantity, i.UnitPrice, discountPolicy)).ToList();
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            Date = date,
            Customer = customer,
            Branch = branch,
            Cancelled = false,
            Items = saleItems,
            TotalAmount = saleItems.Sum(x => x.Total)
        };
    }
}
