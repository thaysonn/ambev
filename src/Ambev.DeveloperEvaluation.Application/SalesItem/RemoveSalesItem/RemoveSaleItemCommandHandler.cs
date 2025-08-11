using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.RemoveSalesItem
{
    public class RemoveSaleItemCommandHandler : IRequestHandler<RemoveSaleItemCommand, RemoveSaleItemResult>
    {
        private readonly ISaleRepository _repo;
        public RemoveSaleItemCommandHandler(ISaleRepository repo)
        {
            _repo = repo;
        }

        public async Task<RemoveSaleItemResult> Handle(RemoveSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repo.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
                return new RemoveSaleItemResult { Success = false, Errors = new[] { "Sale not found" } };

            var item = sale.Items.FirstOrDefault(x => x.Id == request.ItemId);
            if (item == null)
                return new RemoveSaleItemResult { Success = false, Errors = new[] { "Item not found" } };

            sale.Items.Remove(item);
            sale.TotalAmount = sale.Items.Sum(x => x.Total);
             
            await _repo.SaveChangesAsync(cancellationToken);

            return new RemoveSaleItemResult { Success = true };
        }
    }
}
