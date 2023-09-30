using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Exceptions.Category;
using ECommerce.Core.Filters.Category;
using ECommerce.Core.Helpers;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities;
using ECommerce.Service.Abstract;
using ECommerce.Service.Extensions;
using ECommerce.Service.Helpers;
using ECommerce.Service.Helpers.FilterServices;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Service.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Lazy<DatabaseSaveChanges> _saveChanges;
        private readonly HttpContext _httpContext;
        private IRepositoryBase<Category> Categories
            => _unitOfWork.GetRepository<Category>();

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _saveChanges = new Lazy<DatabaseSaveChanges>(() => new DatabaseSaveChanges(unitOfWork));
            _httpContext = httpContext.HttpContext;
        }

        public async Task<bool> AddCategoryAsync(CategoryAddDto categoryDto)
        {
            Category categoryExists = await GetCategoryByName(categoryDto.CategoryName);
            if (categoryExists != null)
                if (categoryExists.DeletedDate != null)
                {
                    categoryExists.DeletedDate = null;
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
                else
                    throw new CategoryAlreadyExistsException();

            Category category = _mapper.Map<Category>(categoryDto);
            await Categories.AddAsync(category);
            await _saveChanges.Value.SaveChangesAsync();

            return true;
        }

        public CategoryResponseFilter<List<CategoryListDto>> GetCategories(CategoryRequestFilter filters)
        {
            var categories = Categories.GetAll(_ => _.DeletedDate == null);
            CategoryResponseFilter<List<CategoryListDto>> response = new CategoryFilterService(_mapper).FilterCategories(categories, filters);
            new AddHeadersToResponse(_httpContext).AddToResponse(response.Headers);

            return response;
        }

        public async Task<CategoryListDto> GetCategoryByIdAsync(string id)
        {
            Category category = await GetCategoryById(id);
            return _mapper.Map<CategoryListDto>(category);
        }

        public async Task<bool> RemoveCategoryAsync(string id)
        {
            Category category = await GetCategoryById(id);
            category.DeletedDate = DateTime.Now;
            category.DeletedBy = _httpContext.User.GetUserName();
            Categories.SafeRemove(category);
            await _saveChanges.Value.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryUpdateDto categoryDto)
        {
            Category categoryExists = await GetCategoryByName(categoryDto.CategoryName);
            if (categoryExists != null)
            {
                if (categoryExists.DeletedDate is null)
                    throw new CategoryAlreadyExistsException();

                categoryExists.DeletedDate = null;
                await _unitOfWork.SaveChangesAsync();
            }
            Category category = await GetCategoryById(categoryDto.Id.ToString());
            category.ModifiedDate = DateTime.Now;
            _mapper.Map(categoryDto, category);
            await _saveChanges.Value.SaveChangesAsync();

            return true;
        }

        public async Task<Category> GetCategoryByName(string categoryName)
            => await Categories.GetFirstWhereAsync(_ => _.CategoryName.ToLower() == categoryName.ToLower());

        private async Task<Category> GetCategoryById(string id)
        {
            Category category = await Categories.GetByIdAsync(id);
            if (category is null)
                throw new CategoryNotFoundException();

            return category;
        }
    }
}
