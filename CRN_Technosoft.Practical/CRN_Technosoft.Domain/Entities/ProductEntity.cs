
namespace CRN_Technosoft.Domain.Entities
{
    public sealed class ProductEntity
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }
        public required string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public ICollection<ItemEntity> Items { get; set; }
        = new List<ItemEntity>();
    }
}