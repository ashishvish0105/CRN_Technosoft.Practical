using AutoMapper;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Domain.Entities;

namespace CRN_Technosoft.Application.Mapping;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ProductEntity, GetProductDTO>();
        CreateMap<CreateProductDTO, ProductEntity>();
        CreateMap<ProductEntity, CreateProductDTO>();
        CreateMap<EditProductDTO, ProductEntity>();
    }
}
