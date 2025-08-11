using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using FluentValidation.TestHelper;
using Xunit;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validation;

public class UpdateSaleCommandValidatorTests
{
    private readonly UpdateSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Valid UpdateSaleCommand should pass all validation rules")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty Id should fail validation")]
    public void Given_EmptyId_When_Validated_Then_ShouldHaveError()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.Empty,
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory(DisplayName = "Invalid SaleNumber should fail validation")]
    [InlineData("")]
    public void Given_InvalidSaleNumber_When_Validated_Then_ShouldHaveError(string saleNumber)
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    [Fact(DisplayName = "SaleNumber longer than max length should fail validation")]
    public void Given_SaleNumberLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = new string('A', 51),
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    [Fact(DisplayName = "Future date should fail validation")]
    public void Given_FutureDate_When_Validated_Then_ShouldHaveError()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S123",
            Date = DateTime.UtcNow.AddDays(1),
            Customer = "Customer Name",
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Theory(DisplayName = "Invalid Customer should fail validation")]
    [InlineData("")]
    public void Given_InvalidCustomer_When_Validated_Then_ShouldHaveError(string customer)
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = customer,
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Customer);
    }

    [Fact(DisplayName = "Customer longer than max length should fail validation")]
    public void Given_CustomerLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = new string('C', 201),
            Branch = "Branch Name"
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Customer);
    }

    [Theory(DisplayName = "Invalid Branch should fail validation")]
    [InlineData("")]
    public void Given_InvalidBranch_When_Validated_Then_ShouldHaveError(string branch)
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = branch
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Branch);
    }

    [Fact(DisplayName = "Branch longer than max length should fail validation")]
    public void Given_BranchLongerThanMax_When_Validated_Then_ShouldHaveError()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S123",
            Date = DateTime.UtcNow,
            Customer = "Customer Name",
            Branch = new string('B', 201)
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Branch);
    }
}
