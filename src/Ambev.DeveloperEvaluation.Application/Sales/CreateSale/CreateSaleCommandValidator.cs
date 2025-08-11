using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required.")
            .MaximumLength(50);

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Date cannot be in the future.");

        RuleFor(x => x.Customer)
            .NotEmpty().WithMessage("Customer is required.")
            .MaximumLength(200);

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage("Branch is required.")
            .MaximumLength(200);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");
         
        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.Product).Distinct().Count() == items.Count)
            .WithMessage("Duplicate products are not allowed.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemValidator());
    }

    private sealed class CreateSaleItemValidator : AbstractValidator<CreateSaleItemResult>
    {
        public CreateSaleItemValidator()
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