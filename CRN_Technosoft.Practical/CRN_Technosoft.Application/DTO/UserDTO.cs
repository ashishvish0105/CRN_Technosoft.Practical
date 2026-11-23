using System.ComponentModel.DataAnnotations;

namespace CRN_Technosoft.Application.DTO
{
    public struct LoginUserDTO
    {
        public string? UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public struct AddUserDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public required string Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")]
        public required string Password { get; set; }
    }

    public struct EditUserDTO
    {
        public required Guid Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public required string Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")]
        public required string Password { get; set; }
    }

    public struct GetUserDTO
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public GetUserDTO() { }
    }
}
