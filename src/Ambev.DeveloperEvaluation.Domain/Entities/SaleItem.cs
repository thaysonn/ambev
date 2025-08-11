using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public bool Cancelled { get; set; }

    public void UpdateValues(int quantity, decimal unitPrice)
    {
        if (quantity > 20) throw new InvalidOperationException("Cannot sell more than 20 units");
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = CalculateDiscount(quantity, unitPrice);
        Total = CalculateTotal(quantity, unitPrice);
    }

    public static decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (quantity > 20) throw new InvalidOperationException("Cannot sell more than 20 units");
        if (quantity >= 10) return Money.Round2(quantity * unitPrice * 0.20m);
        if (quantity >= 4) return Money.Round2(quantity * unitPrice * 0.10m);
        return 0m;
    }

    public static decimal CalculateTotal(int quantity, decimal unitPrice)
    {
        var gross = quantity * unitPrice;
        var discount = CalculateDiscount(quantity, unitPrice);
        return Money.Round2(gross - discount);
    }
}
