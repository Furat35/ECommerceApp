using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Filters;
using ECommerce.Core.Filters.Category;
using ECommerce.Core.HeaderResponses;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Helpers.FilterServices
{
    public class CategoryFilterService
    {
        private readonly IMapper _mapper;

        public CategoryFilterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public CategoryResponseFilter<List<CategoryListDto>> FilterCategories(IQueryable<Category> categories, CategoryRequestFilter filters)
        {
            if (filters.Includes != null)
                categories = categories.Where(_ => _.CategoryName.ToLower().Contains(filters.Includes.ToLower()));

            Metadata metadata = new()
            {
                PageSize = filters.Size,
                TotalEntities = categories.Count(),
                TotalPages = categories.Count() / filters.Size,
                CurrentPage = filters.Page
            };

            List<Category> filteredCategories = categories.Skip(metadata.PageSize * metadata.CurrentPage)
                .Take(filters.Size)
                .ToList();

            return new()
            {
                ResponseValue = _mapper.Map<List<CategoryListDto>>(filteredCategories),
                Headers = new CustomHeaderResponse().AddPaginationHeader(metadata)
            };
        }
    }
}
