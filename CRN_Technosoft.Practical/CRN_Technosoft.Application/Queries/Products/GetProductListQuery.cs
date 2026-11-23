
using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Products
{
    public record GetProductListQuery(int pageIndex,int totalLength) :IRequest<List<GetProductDTO>>;
}
