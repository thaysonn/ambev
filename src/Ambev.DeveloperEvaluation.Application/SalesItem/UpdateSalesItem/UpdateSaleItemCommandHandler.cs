using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Policies;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.UpdateSalesItem
{
    public class UpdateSaleItemCommandHandler : IRequestHandler<UpdateSaleItemCommand, UpdateSaleItemResult>
    {
        private readonly ISaleRepository _repo;
        private readonly IDiscountPolicy _discountPolicy;
        public UpdateSaleItemCommandHandler(ISaleRepository repo, IDiscountPolicy discountPolicy)
        {
            _repo = repo;
            _discountPolicy = discountPolicy;
        }

        public async Task<UpdateSaleItemResult> Handle(UpdateSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repo.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
                return new UpdateSaleItemResult { Success = false, Errors = new[] { "Sale not found" } };

            var item = sale.Items.FirstOrDefault(x => x.Id == request.ItemId);
            if (item == null)
                return new UpdateSaleItemResult { Success = false, Errors = new[] { "Item not found" } };

            if (request.Quantity > 20)
                return new UpdateSaleItemResult { Success = false, Errors = new[] { "Cannot sell more than 20 units" } };

            var updated = SaleItem.Create(item.Product, request.Quantity, request.UnitPrice, _discountPolicy);
            item.Quantity = updated.Quantity;
            item.UnitPrice = updated.UnitPrice;
            item.Discount = updated.Discount;
            item.Total = updated.Total;

            sale.TotalAmount = sale.Items.Sum(x => x.Total);

            await _repo.UpdateAsync(sale, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            return new UpdateSaleItemResult { Success = true };
        }
    }
}
