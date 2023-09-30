using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Mappings.AutoMapperProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryAddDto, Category>();
            CreateMap<Category, CategoryListDto>();
            CreateMap<CategoryUpdateDto, CategoryListDto>();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<CategoryListDto, Category>();
        }
    }
}
