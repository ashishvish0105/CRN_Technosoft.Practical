using CRN_Technosoft.Application.DTO;

namespace CRN_Technosoft.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        public Task<(bool Success, int UserId)> ValidateUserAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);
        public Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken cancellationToken);
    }
}
