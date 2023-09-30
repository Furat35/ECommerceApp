using ECommerce.Core.ActionFilters;
using ECommerce.Core.Consts;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Filters.Product;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Moderator},{RoleConsts.Admin}")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        [AddCache]
        [AllowAnonymous]
        public IActionResult GetAllProducts([FromQuery] ProductRequestFilter filters = null)
        {
            ProductResponseFilter<List<ProductListDto>> products = _productService.GetProducts(filters);
            return Ok(products.ResponseValue);
        }

        /// <summary>
        /// Get product with the given id
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromRoute] string id)
        {
            ProductListDto product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        /// <summary>
        /// Add a product
        /// </summary>
        [HttpPost]
        [RemoveCache(CacheActionName = nameof(GetAllProducts))]
        public async Task<IActionResult> AddProduct([FromBody] ProductAddDto productDto)
        {
            await _productService.AddProductAsync(productDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        [HttpPut]
        [RemoveCache(CacheActionName = nameof(GetAllProducts))]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDto productDto)
        {
            await _productService.UpdateProductAsync(productDto);
            return Ok();
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete]
        [RemoveCache(CacheActionName = nameof(GetAllProducts))]
        public async Task<IActionResult> DeleteProduct([FromQuery] string id)
        {
            await _productService.RemoveProductAsync(id);
            return Ok();
        }
    }
}
