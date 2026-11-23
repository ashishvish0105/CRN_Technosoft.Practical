using CRN_Technosoft.Application.DTO;
using MediatR;


namespace CRN_Technosoft.Application.Command.Products
{
    public record AddProductCommand(CreateProductDTO addProductDTO, string createdBy):IRequest<GetProductDTO>;
}
