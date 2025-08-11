using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _repo;
    public UpdateSaleCommandHandler(ISaleRepository repo)
    {
        _repo = repo;
    }

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

        sale.AddDomainEvent(new SaleUpdatedEvent(sale));
        await _repo.UpdateAsync(sale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken); 

        return new UpdateSaleResult { Success = true };
    }
}
