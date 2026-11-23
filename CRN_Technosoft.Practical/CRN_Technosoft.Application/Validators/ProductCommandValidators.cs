using CRN_Technosoft.Application.Command.Products;
using FluentValidation;

namespace CRN_Technosoft.Application.Validators;

public sealed class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(command => command.addProductDTO.ProductName)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(command => command.createdBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class EditProductCommandValidator : AbstractValidator<EditProductCommand>
{
    public EditProductCommandValidator()
    {
        RuleFor(command => command.productId)
            .GreaterThan(0);
        RuleFor(command => command.EditProductDTO.ProductName)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(command => command.modifiedBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}
