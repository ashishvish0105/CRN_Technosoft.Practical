using CRN_Technosoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CRN_Technosoft.Infrastructure.Repositories
{

    public sealed class ItemRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _memoryCache;

        private const string ProductListCacheKey = "ProductList";

        private static long _productListCacheVersion;

        public ItemRepository(
            ApplicationDbContext applicationDbContext,
            IMemoryCache memoryCache)
        {
            _applicationDbContext = applicationDbContext;
            _memoryCache = memoryCache;
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
    }
}