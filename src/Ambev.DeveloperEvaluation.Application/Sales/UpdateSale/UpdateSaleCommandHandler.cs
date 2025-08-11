using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _repo;
    public UpdateSaleCommandHandler(ISaleRepository repo) { _repo = repo; }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
        {
            return new UpdateSaleResult { Success = false, Errors = new[] { "Sale not found" } };
        }

        sale.SaleNumber = request.SaleNumber;
        sale.Date = request.Date;
        sale.Customer = request.Customer;
        sale.Branch = request.Branch;
        sale.Items = request.Items.Select(i => new SaleItem
        {
            Id = Guid.NewGuid(),
            Product = i.Product,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            Discount = CalculateDiscount(i.Quantity, i.UnitPrice),
            Total = CalculateTotal(i.Quantity, i.UnitPrice),
            Cancelled = false
        }).ToList();
        sale.TotalAmount = sale.Items.Sum(x => x.Total);

        sale.AddDomainEvent(new SaleUpdatedEvent(sale));
        await _repo.UpdateAsync(sale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken); 

        return new UpdateSaleResult { Success = true };
    }

    private decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (quantity >= 10 && quantity <= 20)
            return quantity * unitPrice * 0.2m;
        if (quantity >= 4)
            return quantity * unitPrice * 0.1m;
        return 0m;
    }

    private decimal CalculateTotal(int quantity, decimal unitPrice)
    {
        var discount = CalculateDiscount(quantity, unitPrice);
        return quantity * unitPrice - discount;
    }
}

public class UpdateSaleResult
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
