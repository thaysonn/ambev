using Ambev.DeveloperEvaluation.Application.SalesItem.UpdateSalesItem;
using FluentValidation.TestHelper;
using Xunit;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Application.SalesItem.Validation;

public class UpdateSaleItemCommandValidatorTests
{
    private readonly UpdateSaleItemCommandValidator _validator = new();

    [Fact(DisplayName = "Valid UpdateSaleItemCommand should pass all validation rules")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = new UpdateSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Product = "Beer",
            Quantity = 2,
            UnitPrice = 10
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid Product should fail validation")]
    [InlineData("")]
    public void Given_InvalidProduct_When_Validated_Then_ShouldHaveError(string product)
    {
        var command = new UpdateSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Product = product,
            Quantity = 2,
            UnitPrice = 10
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    [Fact(DisplayName = "Product longer than max length should fail validation")]
    public void Given_ProductLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new UpdateSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Product = new string('P', 201),
            Quantity = 2,
            UnitPrice = 10
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    [Theory(DisplayName = "Invalid Quantity should fail validation")]
    [InlineData(0)]
    [InlineData(21)]
    public void Given_InvalidQuantity_When_Validated_Then_ShouldHaveError(int quantity)
    {
        var command = new UpdateSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Product = "Beer",
            Quantity = quantity,
            UnitPrice = 10
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory(DisplayName = "Invalid UnitPrice should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidUnitPrice_When_Validated_Then_ShouldHaveError(decimal unitPrice)
    {
        var command = new UpdateSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Product = "Beer",
            Quantity = 2,
            UnitPrice = unitPrice
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }
}
