using System.ComponentModel.DataAnnotations;

namespace CRN_Technosoft.Application.DTO
{
    public class LoginResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; init; }
        public User user { get; set; } = new User();
    }

    public sealed class RefreshTokenRequestDto
    {
        [Required]
        [StringLength(256, MinimumLength = 32)]
        public required string RefreshToken { get; set; }
    }
    public class User
    {
        public int UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public List<string> Roles { get; init; } = new List<string>();
    }
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(254)]
        public required string Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 1)]
        public required string Password { get; set; }
    }

}
