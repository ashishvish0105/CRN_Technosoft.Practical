using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Users
{
    public record GetUserListQuery(int PageIndex = 1, int PageSize = 10):IRequest<List<GetUserDTO>>;
}
