using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CRN_Technosoft.Infrastructure.Repositories
{

    public sealed class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ItemRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ItemEntity> AddItem(
            ItemEntity item,
            CancellationToken cancellationToken)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var productExists = await _applicationDbContext.Products.AnyAsync(x => x.Id == item.ProductId, cancellationToken);

            if (!productExists)
                throw new InvalidOperationException("Product does not exist.");

            _applicationDbContext.Items.Add(item);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return item;
        }

        public async Task<bool> EditItem(
            int itemId,
            ItemEntity item,
            CancellationToken cancellationToken)
        {
            if (itemId <= 0 || item == null)
                return false;

            var productExists = await _applicationDbContext.Products
                .AnyAsync(x => x.Id == item.ProductId, cancellationToken);

            if (!productExists)
                throw new InvalidOperationException("Product does not exist.");

            var affectedRows = await _applicationDbContext.Items
                .Where(x => x.Id == itemId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ProductId, item.ProductId)
                    .SetProperty(x => x.Quantity, item.Quantity),
                    cancellationToken);

            return affectedRows > 0;
        }

        public async Task<bool> DeleteItem(
            int itemId,
            CancellationToken cancellationToken = default)
        {
            if (itemId <= 0)
                return false;

            var affectedRows = await _applicationDbContext.Items
                .Where(x => x.Id == itemId)
                .ExecuteDeleteAsync(cancellationToken);

            return affectedRows > 0;
        }

        public async Task<List<ItemEntity>> GetItemList(
            int productId,
            CancellationToken cancellationToken = default)
        {
            return await _applicationDbContext.Items
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => new ItemEntity
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ItemEntity?> GetItemById(
            int itemId,
            CancellationToken cancellationToken = default)
        {
            if (itemId <= 0)
                return null;

            return await _applicationDbContext.Items
                .AsNoTracking()
                .Where(x => x.Id == itemId)
                .Select(x => new ItemEntity
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}