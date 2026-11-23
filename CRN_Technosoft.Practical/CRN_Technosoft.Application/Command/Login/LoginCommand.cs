using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Command.Login
{
    public record class LoginCommand(LoginRequestDto loginUserDTO):IRequest<LoginResponseDto>;

}
