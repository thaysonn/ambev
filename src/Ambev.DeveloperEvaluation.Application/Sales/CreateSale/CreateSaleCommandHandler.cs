using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Policies;
using MediatR;
using static Ambev.DeveloperEvaluation.Domain.ValueObjects.Money;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
 
public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _repo;
    private readonly IMediator _mediator;
    private readonly IDiscountPolicy _discountPolicy;
    public CreateSaleCommandHandler(ISaleRepository repo, IMediator mediator, IDiscountPolicy discountPolicy)
    {
        _repo = repo;
        _mediator = mediator;
        _discountPolicy = discountPolicy;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    { 
        foreach (var item in request.Items)
        {
            if (item.Quantity > 20)
                return new CreateSaleResult { Success = false, Errors = new[] { $"Cannot sell more than 20 units of {item.Product}" } };
        }
         
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = request.SaleNumber,
            Date = request.Date,
            Customer = request.Customer,
            Branch = request.Branch,
            Cancelled = false,
            Items = request.Items.Select(i => {
                var saleItem = new SaleItem
                {
                    Id = Guid.NewGuid(),
                    Product = i.Product,
                    Cancelled = false
                };
                var percent = _discountPolicy.GetPercent(i.Quantity);
                var discount = Round2(i.Quantity * i.UnitPrice * percent);
                saleItem.Quantity = i.Quantity;
                saleItem.UnitPrice = i.UnitPrice;
                saleItem.Discount = discount;
                saleItem.Total = Round2(i.Quantity * i.UnitPrice - discount);
                return saleItem;
            }).ToList()
        };
        sale.TotalAmount = sale.Items.Sum(x => x.Total);

        sale.AddDomainEvent(new SaleCreatedEvent(sale));
        await _repo.AddAsync(sale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return new CreateSaleResult { Success = true, SaleId = sale.Id, SaleNumber = sale.SaleNumber };
    }
}
