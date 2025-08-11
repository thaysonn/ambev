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
                saleItem.UpdateValues(i.Quantity, i.UnitPrice);
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
