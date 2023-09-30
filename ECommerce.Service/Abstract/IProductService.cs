using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Filters.Product;

namespace ECommerce.Service.Abstract
{
    public interface IProductService
    {
        ProductResponseFilter<List<ProductListDto>> GetProducts(ProductRequestFilter filters = null);
        Task<ProductListDto> GetProductByIdAsync(string id);
        Task<bool> AddProductAsync(ProductAddDto productDto);
        Task<bool> RemoveProductAsync(string id);
        Task<bool> UpdateProductAsync(ProductUpdateDto productDto);
    }
}
