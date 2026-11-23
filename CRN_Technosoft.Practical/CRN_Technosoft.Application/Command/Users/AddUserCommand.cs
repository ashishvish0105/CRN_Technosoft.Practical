using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Command.Users
{
    public record AddUserCommand(AddUserDTO addUserDTO):IRequest<int>;
}
