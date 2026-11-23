using AutoMapper;
using CRN_Technosoft.Application.Command.Products;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;


namespace CRN_Technosoft.Application.Handlers.Products
{
    public class AddProductHandler(IProductRepository _productRepository, IMapper _mapper) : IRequestHandler<AddProductCommand, GetProductDTO>
    {
        public async Task<GetProductDTO> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = _mapper.Map<ProductEntity>(request.addProductDTO);
            productEntity.CreatedBy = request.createdBy;

            var savedEntity = await _productRepository.AddProduct(productEntity, cancellationToken);

            var response = _mapper.Map<GetProductDTO>(savedEntity);

            return response;
        }
    }
}
