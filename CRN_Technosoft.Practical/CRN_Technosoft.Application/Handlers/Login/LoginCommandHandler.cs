
using CRN_Technosoft.Application.Command.Login;
using CRN_Technosoft.Application.Common.Interfaces;
using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Login
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(
            IIdentityService identityService,
            IJwtTokenService jwtTokenService)
        {
            _identityService = identityService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            if (request?.loginUserDTO == null)
            {
                throw new ArgumentNullException(
                    nameof(request),
                    "Request or loginUserDTO cannot be null.");
            }

            var result = await _identityService.ValidateUserAsync(
                request.loginUserDTO,
                cancellationToken);

            if (!result.Success)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var response = await _jwtTokenService.CreateTokenAsync(
                result.UserId,
                cancellationToken);

            return response;
        }
    }
}
