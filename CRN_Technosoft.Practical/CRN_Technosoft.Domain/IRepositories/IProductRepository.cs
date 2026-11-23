using CRN_Technosoft.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRN_Technosoft.Domain.IRepositories
{
    public interface IProductRepository
    {
        public Task<ProductEntity> AddProduct(ProductEntity product, CancellationToken cancellationToken = default);
        public Task<bool> EditProduct(int productId, ProductEntity product, CancellationToken cancellationToken = default);
        public Task<bool> DeleteProduct(int productId, CancellationToken cancellationToken = default);
        public Task<List<ProductEntity>> GetProductList(int pageIndex, int totalLength, CancellationToken cancellationToken = default);
        public Task<ProductEntity?> GetProductById(int ProductId, CancellationToken cancellationToken = default);
    }
}
