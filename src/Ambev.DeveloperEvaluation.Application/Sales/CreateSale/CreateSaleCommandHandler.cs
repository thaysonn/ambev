using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
 
public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _repo;
    private readonly IMediator _mediator;
    public CreateSaleCommandHandler(ISaleRepository repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Business rules for discounts
        foreach (var item in request.Items)
        {
            if (item.Quantity > 20)
                return new CreateSaleResult { Success = false, Errors = new[] { $"Cannot sell more than 20 units of {item.Product}" } };
        }

        // Map to domain entity
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = request.SaleNumber,
            Date = request.Date,
            Customer = request.Customer,
            Branch = request.Branch,
            Cancelled = false,
            Items = request.Items.Select(i => new SaleItem
            {
                Id = Guid.NewGuid(),
                Product = i.Product,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = CalculateDiscountAmount(i.Quantity, i.UnitPrice),
                Total = CalculateTotal(i.Quantity, i.UnitPrice),
                Cancelled = false
            }).ToList()
        };
        sale.TotalAmount = sale.Items.Sum(x => x.Total);

        sale.AddDomainEvent(new SaleCreatedEvent(sale));
        await _repo.AddAsync(sale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return new CreateSaleResult { Success = true, SaleId = sale.Id, SaleNumber = sale.SaleNumber };
    }

    private decimal CalculateDiscountAmount(int qty, decimal unitPrice)
    {
        if (qty > 20) throw new InvalidOperationException("..."); // não deve acontecer por causa do validator
        if (qty >= 10) return Math.Round(qty * unitPrice * 0.20m, 2, MidpointRounding.AwayFromZero);
        if (qty >= 4) return Math.Round(qty * unitPrice * 0.10m, 2, MidpointRounding.AwayFromZero);
        return 0m;
    }

    private decimal CalculateTotal(int qty, decimal unitPrice)
    {
        var gross = qty * unitPrice;
        var discount = CalculateDiscountAmount(qty, unitPrice);
        return Math.Round(gross - discount, 2, MidpointRounding.AwayFromZero);
    }
}
