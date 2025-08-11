using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.SaleNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Date).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5));
        RuleFor(x => x.Customer).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Branch).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemValidator());
    }

    private sealed class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemDto>
    {
        public UpdateSaleItemValidator()
        {
            // Id pode ser nulo para novos itens
            RuleFor(i => i.Product).NotEmpty().MaximumLength(200);
            RuleFor(i => i.Quantity).InclusiveBetween(1, 20);
            RuleFor(i => i.UnitPrice).GreaterThan(0m);
        }
    }
}