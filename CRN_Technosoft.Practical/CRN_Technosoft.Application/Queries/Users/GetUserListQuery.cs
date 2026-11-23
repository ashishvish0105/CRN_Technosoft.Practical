using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Users
{
    public record GetUserListQuery:IRequest<List<GetUserDTO>>;
}
