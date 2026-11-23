namespace CRN_Technosoft.Domain.Entities
{
    public class ItemEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductEntity? Product { get; set; }
    }

}