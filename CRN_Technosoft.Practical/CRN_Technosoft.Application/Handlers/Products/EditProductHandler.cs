using AutoMapper;
using CRN_Technosoft.Application.Command.Products;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Products
{
    public sealed class EditProductHandler(
        IProductRepository productRepository,
        IMapper mapper) : IRequestHandler<EditProductCommand, bool>
    {
        public async Task<bool> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = mapper.Map<ProductEntity>(request.EditProductDTO);
            productEntity.Id = request.productId;

            return await productRepository.EditProduct(
                request.productId,
                productEntity,
                cancellationToken);
        }
    }
}
