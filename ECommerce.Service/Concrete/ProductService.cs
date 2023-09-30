using AutoMapper;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Exceptions.Product;
using ECommerce.Core.Filters.Product;
using ECommerce.Core.Helpers;
using ECommerce.Core.RabbitMqMessageBroker.Abstract;
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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Lazy<DatabaseSaveChanges> _saveChanges;
        private readonly HttpContext _httpContext;
        private IRepositoryBase<Product> Products
            => _unitOfWork.GetRepository<Product>();

        private static readonly IRabbitMqMessagePublisher _messagePublisherService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _saveChanges = new Lazy<DatabaseSaveChanges>(() => new DatabaseSaveChanges(unitOfWork));
            _httpContext = httpContext.HttpContext;
        }

        //static ProductService()
        //{
        //    _messagePublisherService = new RabbitMqMessagePublisher();
        //    _messagePublisherService.Connect("E_Commerce_Orders", "E.Commerce.Orders.Completed", ExchangeType.Direct,  "E.Commerce.Orders.Completed.Key");
        //}

        public ProductResponseFilter<List<ProductListDto>> GetProducts(ProductRequestFilter filters = null)
        {
            var products = Products.GetAll(_ => _.DeletedDate == null, includeProperties: new()
            {
               _ => _.Categories
            });
            ProductResponseFilter<List<ProductListDto>> response = new ProductFilterService(_mapper).FilterProducts(products, filters);
            new AddHeadersToResponse(_httpContext).AddToResponse(response.Headers);

            return response;
        }

        public async Task<ProductListDto> GetProductByIdAsync(string id)
        {
            Product product = await Products.GetByIdAsync(id, new()
            {
                _ => _.Categories
            });
            if (product is null)
                throw new ProductNotFoundException();

            return _mapper.Map<ProductListDto>(product);
        }

        public async Task<bool> RemoveProductAsync(string id)
        {
            Product product = await GetProductById(id);
            product.DeletedDate = DateTime.Now;
            product.DeletedBy = _httpContext.User.GetUserName();
            Products.SafeRemove(product);
            await _saveChanges.Value.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddProductAsync(ProductAddDto productDto)
        {
            Product product = _mapper.Map<Product>(productDto);
            await AddCategoriesToProductAsync(productDto.CategoryIds, product);
            await Products.AddAsync(product);
            await _saveChanges.Value.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateProductAsync(ProductUpdateDto productDto)
        {
            Product product = await GetProductById(productDto.Id.ToString());
            await RemoveCategoriesFromProductAsync(product.Id.ToString());
            await AddCategoriesToProductAsync(productDto.CategoryIds, product);
            product.ModifiedDate = DateTime.Now;
            _mapper.Map(productDto, product);
            await _saveChanges.Value.SaveChangesAsync();

            return true;
        }

        public async Task RemoveCategoriesFromProductAsync(string ProductId)
        {
            Product product = await Products.GetByIdAsync(ProductId, new()
            {
                _ => _.Categories
            });
            product.Categories.Clear();
            await _saveChanges.Value.SaveChangesAsync();
        }

        private async Task<Product> GetProductById(string id)
        {
            Product product = await Products.GetByIdAsync(id);
            if (product is null)
                throw new ProductNotFoundException();

            return product;
        }

        private async Task AddCategoriesToProductAsync(ICollection<string> categoryIds, Product product)
        {
            foreach (var categoryId in categoryIds)
            {
                Category category = await _unitOfWork.GetRepository<Category>()
                    .GetByIdAsync(categoryId);
                if (category != null)
                    product.Categories.Add(category);
            }
        }
    }
}
