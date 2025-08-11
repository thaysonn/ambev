using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Policies;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.CreateSalesItem
{
    public class AddSaleItemCommandHandler : IRequestHandler<AddSaleItemCommand, AddSaleItemResult>
    {
        private readonly ISaleRepository _repo;
        private readonly IDiscountPolicy _discountPolicy;
        public AddSaleItemCommandHandler(ISaleRepository repo, IDiscountPolicy discountPolicy)
        {
            _repo = repo;
            _discountPolicy = discountPolicy;
        }

        public async Task<AddSaleItemResult> Handle(AddSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repo.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
                return new AddSaleItemResult { Success = false, Errors = new[] { "Sale not found" } };

            if (request.Quantity > 20)
                return new AddSaleItemResult { Success = false, Errors = new[] { $"Cannot sell more than 20 units of {request.Product}" } };

            var saleItem = SaleItem.Create(request.Product, request.Quantity, request.UnitPrice, _discountPolicy);
            sale.Items.Add(saleItem);
            sale.TotalAmount = sale.Items.Sum(x => x.Total);

            await _repo.SaveChangesAsync(cancellationToken);

            return new AddSaleItemResult { Success = true, ItemId = saleItem.Id };
        }
    }
}
