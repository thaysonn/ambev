using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validation;

public class CreateSaleItemValidatorTests
{
    private readonly object _validator = null!;

    public CreateSaleItemValidatorTests()
    {
        // CreateSaleItemValidator is private, so we instantiate via CreateSaleCommandValidator
        var parentValidator = new CreateSaleCommandValidator();
        var field = typeof(CreateSaleCommandValidator).GetNestedType("CreateSaleItemValidator", System.Reflection.BindingFlags.NonPublic);
        _validator = Activator.CreateInstance(field!);
    }

    [Fact(DisplayName = "Valid CreateSaleItemCommand should pass all validation rules")]
    public void Given_ValidItem_When_Validated_Then_ShouldNotHaveErrors()
    {
        var item = new CreateSaleItemCommand { Product = "Beer", Quantity = 2, UnitPrice = 10 };
        var result = ((FluentValidation.IValidator<CreateSaleItemCommand>)_validator).TestValidate(item);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid Product should fail validation")]
    [InlineData("")]
    public void Given_InvalidProduct_When_Validated_Then_ShouldHaveError(string product)
    {
        var item = new CreateSaleItemCommand { Product = product, Quantity = 2, UnitPrice = 10 };
        var result = ((FluentValidation.IValidator<CreateSaleItemCommand>)_validator).TestValidate(item);
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    [Fact(DisplayName = "Product longer than max length should fail validation")]
    public void Given_ProductLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var item = new CreateSaleItemCommand { Product = new string('P', 201), Quantity = 2, UnitPrice = 10 };
        var result = ((FluentValidation.IValidator<CreateSaleItemCommand>)_validator).TestValidate(item);
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    [Theory(DisplayName = "Invalid Quantity should fail validation")]
    [InlineData(0)]
    [InlineData(21)]
    public void Given_InvalidQuantity_When_Validated_Then_ShouldHaveError(int quantity)
    {
        var item = new CreateSaleItemCommand { Product = "Beer", Quantity = quantity, UnitPrice = 10 };
        var result = ((FluentValidation.IValidator<CreateSaleItemCommand>)_validator).TestValidate(item);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory(DisplayName = "Invalid UnitPrice should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidUnitPrice_When_Validated_Then_ShouldHaveError(decimal unitPrice)
    {
        var item = new CreateSaleItemCommand { Product = "Beer", Quantity = 2, UnitPrice = unitPrice };
        var result = ((FluentValidation.IValidator<CreateSaleItemCommand>)_validator).TestValidate(item);
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }
}
