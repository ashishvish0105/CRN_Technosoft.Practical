using CRN_Technosoft.Application.Command.Products;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Validators;
using Xunit;

namespace CRN_Technosoft.Tests.Products;

public sealed class ProductCommandValidatorTests
{
    [Fact]
    public void AddProductCommandValidator_RejectsBlankProductName()
    {
        var validator = new AddProductCommandValidator();
        var command = new AddProductCommand(
            new CreateProductDTO { ProductName = " " },
            "sample-user");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName.Contains("ProductName"));
    }

    [Fact]
    public void AddProductCommandValidator_RejectsCreatorLongerThanDatabaseLimit()
    {
        var validator = new AddProductCommandValidator();
        var command = new AddProductCommand(
            new CreateProductDTO { ProductName = "Keyboard" },
            new string('u', 101));

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "createdBy");
    }
}
