using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Users
{
    public record GetUserQuery(GetUserDTO getUserDTO) : IRequest<GetUserDTO?>;
}
