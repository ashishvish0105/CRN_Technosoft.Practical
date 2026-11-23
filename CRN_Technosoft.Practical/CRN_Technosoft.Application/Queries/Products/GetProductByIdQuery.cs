using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Products
{
    public record GetProductByIdQuery(int Id) : IRequest<GetProductDTO?>;
}
