using AutoMapper;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Products;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Products
{
       public class GetProductByIdHandler: IRequestHandler<GetProductByIdQuery, GetProductDTO?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper)
        {
            this._productRepository = productRepository;
            this._mapper = mapper;
        }
        public async Task<GetProductDTO?> Handle(GetProductByIdQuery request,CancellationToken cancellationToken)
        {
            // Get product by ID
            var product = await _productRepository
                .GetProductById(request.Id, cancellationToken);

            // Check if product exists
            if (product == null)
            {
                return null;
            }

            // Map entity to DTO
            var mappedProduct = _mapper.Map<GetProductDTO>(product);

            return mappedProduct;
        }
    }
}
