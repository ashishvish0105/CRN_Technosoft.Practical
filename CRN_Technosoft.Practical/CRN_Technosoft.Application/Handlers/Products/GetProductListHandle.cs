using AutoMapper;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Products;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Products
{
    public class GetProductListHandle: IRequestHandler<GetProductListQuery, List<GetProductDTO>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductListHandle(IProductRepository productRepository,IMapper mapper) {
            this._productRepository = productRepository;
            this._mapper = mapper;
        }

        public async Task<List<GetProductDTO>> Handle(GetProductListQuery request,CancellationToken cancellationToken){
            var products = await _productRepository.GetProductList(request.pageIndex, request.totalLength, cancellationToken);
            var mapProducts = _mapper.Map<List<GetProductDTO>>(products);
            return mapProducts;
        }
    }
}
