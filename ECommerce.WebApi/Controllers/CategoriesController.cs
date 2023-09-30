using ECommerce.Core.ActionFilters;
using ECommerce.Core.Consts;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Filters.Category;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Moderator},{RoleConsts.Admin}")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [AddCache]
        [AllowAnonymous]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult GetAllCategories([FromQuery] CategoryRequestFilter filters)
        {
            CategoryResponseFilter<List<CategoryListDto>> response = _categoryService.GetCategories(filters);
            return Ok(response.ResponseValue);
        }

        /// <summary>
        /// Get the category with the given id
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories([FromRoute] string id)
        {
            CategoryListDto category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        /// <summary>
        /// Add a category
        /// </summary>
        [HttpPost]
        [RemoveCache(CacheActionName = nameof(GetAllCategories))]
        public async Task<IActionResult> AddCategory(CategoryAddDto categoryDto)
        {
            await _categoryService.AddCategoryAsync(categoryDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Update a category
        /// </summary>
        [HttpPut]
        [RemoveCache(CacheActionName = nameof(GetAllCategories))]
        public async Task<IActionResult> UpdateCategory(CategoryUpdateDto categoryDto)
        {
            await _categoryService.UpdateCategoryAsync(categoryDto);
            return Ok();
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpDelete]
        [RemoveCache(CacheActionName = nameof(GetAllCategories))]
        public async Task<IActionResult> DeleteCategory([FromQuery] string id)
        {
            await _categoryService.RemoveCategoryAsync(id);
            return Ok();
        }
    }
}
