using CRN_Technosoft.Domain.Entities;

namespace CRN_Technosoft.Domain.IRepositories;

public interface IItemRepository
{
    Task<ItemEntity> AddItem(ItemEntity item, CancellationToken cancellationToken = default);
    Task<bool> EditItem(int itemId, ItemEntity item, CancellationToken cancellationToken = default);
    Task<bool> DeleteItem(int itemId, CancellationToken cancellationToken = default);
    Task<List<ItemEntity>> GetItemList(int productId, CancellationToken cancellationToken = default);
    Task<ItemEntity?> GetItemById(int itemId, CancellationToken cancellationToken = default);
}
