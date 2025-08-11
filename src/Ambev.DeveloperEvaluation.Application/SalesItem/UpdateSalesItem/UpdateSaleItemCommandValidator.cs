using Ambev.DeveloperEvaluation.Application.SalesItem.CreateSalesItem;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.SalesItem.UpdateSalesItem
{
    public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
    {
        public UpdateSaleItemCommandValidator()
        {
            RuleFor(i => i.Product)
                .NotEmpty().WithMessage("Product is required.")
                .MaximumLength(200);

            RuleFor(i => i.Quantity)
                .InclusiveBetween(1, 20)
                .WithMessage("Quantity must be between 1 and 20.");

            RuleFor(i => i.UnitPrice)
                .GreaterThan(0m)
                .WithMessage("Unit price must be greater than zero.");
        }
    }
}
