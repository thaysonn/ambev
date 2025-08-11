using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation.TestHelper;
using Xunit;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validation;

public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Valid CreateSaleCommand should pass all validation rules")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand { Product = "Beer", Quantity = 2, UnitPrice = 10 }
            }
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid SaleNumber should fail validation")]
    [InlineData("")]
    public void Given_InvalidSaleNumber_When_Validated_Then_ShouldHaveError(string saleNumber)
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = saleNumber,
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    [Fact(DisplayName = "SaleNumber longer than max length should fail validation")]
    public void Given_SaleNumberLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = new string('A', 51),
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    [Fact(DisplayName = "Future date should fail validation")]
    public void Given_FutureDate_When_Validated_Then_ShouldHaveError()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow.AddDays(1),
            Customer = "Customer Name",
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Theory(DisplayName = "Invalid Customer should fail validation")]
    [InlineData("")]
    public void Given_InvalidCustomer_When_Validated_Then_ShouldHaveError(string customer)
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = customer,
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Customer);
    }

    [Fact(DisplayName = "Customer longer than max length should fail validation")]
    public void Given_CustomerLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = new string('C', 201),
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Customer);
    }

    [Theory(DisplayName = "Invalid Branch should fail validation")]
    [InlineData("")]
    public void Given_InvalidBranch_When_Validated_Then_ShouldHaveError(string branch)
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = branch,
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Branch);
    }

    [Fact(DisplayName = "Branch longer than max length should fail validation")]
    public void Given_BranchLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = new string('B', 201),
            Items = new List<CreateSaleItemCommand> { new() { Product = "Beer", Quantity = 2, UnitPrice = 10 } }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Branch);
    }

    [Fact(DisplayName = "No items should fail validation")]
    public void Given_NoItems_When_Validated_Then_ShouldHaveError()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand>()
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact(DisplayName = "Duplicate products should fail validation")]
    public void Given_DuplicateProducts_When_Validated_Then_ShouldHaveError()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name",
            Items = new List<CreateSaleItemCommand>
            {
                new() { Product = "Beer", Quantity = 2, UnitPrice = 10 },
                new() { Product = "Beer", Quantity = 3, UnitPrice = 12 }
            }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }
}
