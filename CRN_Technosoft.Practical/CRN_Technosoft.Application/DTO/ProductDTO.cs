using System.ComponentModel.DataAnnotations;


namespace CRN_Technosoft.Application.DTO
{
    public struct GetProductDTO
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
    public struct CreateProductDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public required string ProductName { get; set; }
    }

    public struct EditProductDTO
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public required string ProductName { get; set; }
    }
}
