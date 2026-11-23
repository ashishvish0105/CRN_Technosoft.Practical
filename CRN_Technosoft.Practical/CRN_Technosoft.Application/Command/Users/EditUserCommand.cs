using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Command.Users
{
    public record EditUserCommand(Guid userId, EditUserDTO editUserDTO):IRequest<Guid>;
}
