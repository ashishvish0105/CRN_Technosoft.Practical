using MediatR;

namespace CRN_Technosoft.Application.Command.Products
{
    public record DeleteProductCommand(int ProductId) : IRequest<bool>;
}
