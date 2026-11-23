using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CRN_Technosoft.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _memoryCache;

        private const string ProductListCacheKey = "ProductList";

        private static long _productListCacheVersion;

        public ProductRepository(
            ApplicationDbContext applicationDbContext,
            IMemoryCache memoryCache)
        {
            _applicationDbContext = applicationDbContext;
            _memoryCache = memoryCache;
        }

        public async Task<ProductEntity> AddProduct(
            ProductEntity product,
            CancellationToken cancellationToken)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _applicationDbContext.Products.Add(product);

            var returnValue = await _applicationDbContext.SaveChangesAsync(
                cancellationToken);

            if (returnValue > 0)
            {
                Interlocked.Increment(ref _productListCacheVersion);
                return product;
            }

            throw new InvalidOperationException("Failed to add product.");
        }

        public async Task<bool> EditProduct(
            int productId,
            ProductEntity product,
            CancellationToken cancellationToken)
        {
            if (productId <= 0 || product == null)
                return false;

            var affectedRows = await _applicationDbContext.Products
                .Where(x => x.Id == productId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ProductName, product.ProductName)
                    .SetProperty(x => x.ModifiedBy, product.ModifiedBy)
                    .SetProperty(x => x.ModifiedOn, DateTime.UtcNow),
                    cancellationToken);

            if (affectedRows > 0)
            {
                Interlocked.Increment(ref _productListCacheVersion);
            }

            return affectedRows > 0;
        }

        public async Task<bool> DeleteProduct(
            int productId,
            CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
                return false;

            var affectedRows = await _applicationDbContext.Products
                .Where(x => x.Id == productId)
                .ExecuteDeleteAsync(cancellationToken);

            if (affectedRows > 0)
            {
                Interlocked.Increment(ref _productListCacheVersion);
            }

            return affectedRows > 0;
        }

        public async Task<List<ProductEntity>> GetProductList(
            int pageIndex,
            int totalLength,
            CancellationToken cancellationToken = default)
        {
            var safePageIndex = Math.Max(pageIndex, 1);
            var safePageSize = Math.Clamp(totalLength, 1, 100);
            var skip = (int)Math.Min((long)(safePageIndex - 1) * safePageSize, int.MaxValue);

            var cacheKey =
                $"{ProductListCacheKey}_{Volatile.Read(ref _productListCacheVersion)}_{safePageIndex}_{safePageSize}";

            if (_memoryCache.TryGetValue(
                cacheKey,
                out List<ProductEntity>? cachedProducts)
                && cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _applicationDbContext.Products
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(safePageSize)
                .Select(x => new ProductEntity
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedOn = x.ModifiedOn
                })
                .ToListAsync(cancellationToken);

            _memoryCache.Set(
                cacheKey,
                products,
                TimeSpan.FromMinutes(5));

            return products;
        }

        public async Task<ProductEntity?> GetProductById(
            int productId,
            CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
                return null;

            return await _applicationDbContext.Products
                .AsNoTracking()
                .Where(x => x.Id == productId)
                .Select(x => new ProductEntity
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedOn = x.ModifiedOn,

                    Items = x.Items.Select(item => new ItemEntity
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

}
