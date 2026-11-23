
namespace CRN_Technosoft.Domain.Entities
{
    public sealed class ProductEntity
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public ICollection<ItemEntity> Items { get; set; }
        = new List<ItemEntity>();
    }
}