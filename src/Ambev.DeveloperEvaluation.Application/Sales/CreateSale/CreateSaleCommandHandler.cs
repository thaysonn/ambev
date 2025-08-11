using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Policies;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
 
public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _repo; 
    private readonly IDiscountPolicy _discountPolicy;
    public CreateSaleCommandHandler(ISaleRepository repo, IDiscountPolicy discountPolicy)
    {
        _repo = repo; 
        _discountPolicy = discountPolicy;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    { 
        foreach (var item in request.Items)
        {
            if (item.Quantity > 20)
                return new CreateSaleResult { Success = false, Errors = new[] { $"Cannot sell more than 20 units of {item.Product}" } };
        }

        var sale = Sale.Create(
            request.SaleNumber,
            request.Date,
            request.Customer,
            request.Branch,
            request.Items.Select(i => (i.Product, i.Quantity, i.UnitPrice)),
            _discountPolicy
        );

        sale.AddDomainEvent(new SaleCreatedEvent(sale));
        await _repo.AddAsync(sale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return new CreateSaleResult { Success = true, SaleId = sale.Id, SaleNumber = sale.SaleNumber };
    }
}
