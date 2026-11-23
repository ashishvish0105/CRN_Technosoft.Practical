using CRN_Technosoft.Application.Common.Interfaces;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRN_Technosoft.Infrastructure.Services;

public sealed class JwtTokenService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    IConfiguration configuration) : IJwtTokenService
{
    public async Task<LoginResponseDto> CreateTokenAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByIdAsync(userId.ToString(CultureInfo.InvariantCulture))
            ?? throw new KeyNotFoundException($"User with ID {userId} was not found.");

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("The account is inactive.");
        }

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = CreateAccessToken(user, roles);
        var refreshToken = CreateRefreshToken();
        var refreshExpiresAt = GetRefreshTokenExpiry();

        dbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            TokenHash = HashRefreshToken(refreshToken),
            ExpiresAt = refreshExpiresAt
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateResponse(user, roles, accessToken, accessExpiresAt, refreshToken, refreshExpiresAt);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException("A valid refresh token is required.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var tokenHash = HashRefreshToken(refreshToken);
        var storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("The refresh token is invalid or expired.");
        }

        if (storedToken.RevokedAt is not null)
        {
            var now = DateTime.UtcNow;
            await dbContext.RefreshTokens
                .Where(token => token.UserId == storedToken.UserId && token.RevokedAt == null)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(token => token.RevokedAt, now),
                    cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            throw new UnauthorizedAccessException("The refresh token has already been used.");
        }

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString(CultureInfo.InvariantCulture));
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("The account is not available.");
        }

        var nextRefreshToken = CreateRefreshToken();
        var nextRefreshTokenHash = HashRefreshToken(nextRefreshToken);
        var nowUtc = DateTime.UtcNow;
        var nextRefreshExpiresAt = GetRefreshTokenExpiry();
        var updatedCount = await dbContext.RefreshTokens
            .Where(token =>
                token.Id == storedToken.Id &&
                token.RevokedAt == null &&
                token.ExpiresAt > nowUtc)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(token => token.RevokedAt, nowUtc)
                    .SetProperty(token => token.ReplacedByTokenHash, nextRefreshTokenHash),
                cancellationToken);

        if (updatedCount != 1)
        {
            throw new UnauthorizedAccessException("The refresh token has already been used.");
        }

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = CreateAccessToken(user, roles);
        dbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            TokenHash = nextRefreshTokenHash,
            ExpiresAt = nextRefreshExpiresAt
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreateResponse(
            user,
            roles,
            accessToken,
            accessExpiresAt,
            nextRefreshToken,
            nextRefreshExpiresAt);
    }

    private (string Token, DateTime ExpiresAt) CreateAccessToken(
        ApplicationUser user,
        IEnumerable<string> userRoles)
    {
        var secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is missing.");
        var issuer = configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing.");
        var audience = configuration["JwtSettings:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing.");

        if (!double.TryParse(
                configuration["JwtSettings:ExpiryInMinutes"],
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var expiryMinutes)
            || expiryMinutes is < 1 or > 1440)
        {
            throw new InvalidOperationException(
                "JwtSettings:ExpiryInMinutes must be a number between 1 and 1440.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
        var token = new JwtSecurityTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256)
        });

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private LoginResponseDto CreateResponse(
        ApplicationUser user,
        IEnumerable<string> roles,
        string accessToken,
        DateTime accessExpiresAt,
        string refreshToken,
        DateTime refreshExpiresAt)
    {
        return new LoginResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            user = new User
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            }
        };
    }

    private DateTime GetRefreshTokenExpiry()
    {
        if (!int.TryParse(
                configuration["JwtSettings:RefreshTokenExpiryInDays"],
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var expiryDays)
            || expiryDays is < 1 or > 90)
        {
            throw new InvalidOperationException(
                "JwtSettings:RefreshTokenExpiryInDays must be a number between 1 and 90.");
        }

        return DateTime.UtcNow.AddDays(expiryDays);
    }

    private static string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashRefreshToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
