using CRN_Technosoft.Application.Command.Products;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Products
{
    public sealed class DeleteProductHandler(IProductRepository productRepository)
        : IRequestHandler<DeleteProductCommand, bool>
    {
        public Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return productRepository.DeleteProduct(request.ProductId, cancellationToken);
        }
    }
}
