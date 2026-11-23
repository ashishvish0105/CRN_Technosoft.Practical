using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Command.Products
{
    public record EditProductCommand(int productId, EditProductDTO EditProductDTO) : IRequest<bool>;
}
