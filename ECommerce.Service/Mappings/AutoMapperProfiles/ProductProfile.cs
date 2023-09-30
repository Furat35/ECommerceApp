using AutoMapper;
using ECommerce.Core.DTOs.Product;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductAddDto, Product>();
            CreateMap<Product, ProductListDto>();
            CreateMap<ProductUpdateDto, ProductListDto>();
            CreateMap<ProductUpdateDto, Product>();
        }
    }
}
