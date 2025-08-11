using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _repo;
    public CancelSaleCommandHandler(ISaleRepository repo)
    {
        _repo = repo;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _repo.GetByIdAsync(request.Id, cancellationToken);
       
        if (sale == null)
            return new CancelSaleResult { Success = false, Errors = new[] { "Sale not found" } };
       
        if (sale.Cancelled)
            return new CancelSaleResult { Success = false, Errors = new[] { "Sale already cancelled" } };
       
        sale.Cancelled = true;

        if (sale.Items != null)
            foreach (var item in sale.Items)
                item.Cancelled = true;

        sale.AddDomainEvent(new SaleCanceledEvent(sale));
        await _repo.UpdateAsync(sale, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
        return new CancelSaleResult { Success = true };
    }
}
