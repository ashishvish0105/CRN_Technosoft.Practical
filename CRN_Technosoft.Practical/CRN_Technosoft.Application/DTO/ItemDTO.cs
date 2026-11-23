using System.ComponentModel.DataAnnotations;

namespace CRN_Technosoft.Application.DTO;

public struct ItemDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public struct CreateItemDTO
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}

public struct EditItemDTO
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}
