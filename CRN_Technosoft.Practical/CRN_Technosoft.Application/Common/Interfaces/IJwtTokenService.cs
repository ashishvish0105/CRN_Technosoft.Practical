using CRN_Technosoft.Application.DTO;

namespace CRN_Technosoft.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        Task<LoginResponseDto> CreateTokenAsync(int userId, CancellationToken cancellationToken);

    }
}
