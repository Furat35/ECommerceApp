using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Filters.Category;

namespace ECommerce.Service.Abstract
{
    public interface ICategoryService
    {
        CategoryResponseFilter<List<CategoryListDto>> GetCategories(CategoryRequestFilter filters = null);
        Task<CategoryListDto> GetCategoryByIdAsync(string id);
        Task<bool> AddCategoryAsync(CategoryAddDto categoryDto);
        Task<bool> RemoveCategoryAsync(string id);
        Task<bool> UpdateCategoryAsync(CategoryUpdateDto categoryDto);
    }
}
