using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Command.Users
{
    public record EditUserCommand(int userId, EditUserDTO editUserDTO):IRequest<int>;
}
