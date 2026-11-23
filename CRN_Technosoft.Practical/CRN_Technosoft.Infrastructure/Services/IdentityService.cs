
﻿using CRN_Technosoft.Application.Common.Interfaces;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CRN_Technosoft.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || !user.IsActive)
                return Array.Empty<string>();

            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<(bool Success, int UserId)> ValidateUserAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            if (user == null)
            {
                return (false, 0);
            }

            var validPassword = await _userManager.CheckPasswordAsync(
                user,
                loginRequestDto.Password);

            if (!validPassword)
            {
                return (false, 0);
            }

            return (true, user.Id);
        }

    }
}
