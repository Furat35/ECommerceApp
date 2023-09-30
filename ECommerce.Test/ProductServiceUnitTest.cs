using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.DTOs.Product;
using ECommerce.Core.Exceptions.Product;
using ECommerce.Data.Repositories;
using ECommerce.Data.Repositories.EntityFrameworkContext;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities;
using ECommerce.Service.Abstract;
using ECommerce.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace ECommerce.Test
{
    #region MOCK
    //class MockService : IProductService
    //{
    //    public Task<bool> AddProductAsync(ProductAddDto productDto)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<ProductListDto> GetProductByIdAsync(string id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ProductResponseFilter<List<ProductListDto>> GetProducts(ProductRequestFilter filters = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<bool> RemoveProductAsync(string id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<bool> UpdateProductAsync(ProductUpdateDto productDto)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    #endregion

    public class ProductServiceUnitTest
    {

        public Mock<IProductService> _mockIProductService = new(MockBehavior.Loose);
        public Mock<IUnitOfWork> _mockIUnitOfWork = new(MockBehavior.Loose);
        public Mock<IMapper> _mockIMapper = new(MockBehavior.Loose);
        public Mock<IHttpContextAccessor> _mockIHttpContextAccessor = new(MockBehavior.Loose);
        public Mock<IRepositoryBase<Product>> _mockIRepositoryBase = new(MockBehavior.Loose);
        public EfContext dbContext { get; set; }
        public ProductServiceUnitTest()
        {
            dbContext = CreateMemoryDbContext();
            _mockIUnitOfWork.Setup(_ => _.GetRepository<Product>()).Returns(_mockIRepositoryBase.Object);
        }

        private EfContext CreateMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options;
            var dbContext = new EfContext(options);
            dbContext.Products.AddRange(
                new Product()
                {
                    Id = Guid.Parse("4FC4E4AC-FCF8-468E-821B-48372AF4B9D0"),
                    Categories = new List<Category>() { new Category() { Id = Guid.Parse("ADF91C00-23F0-4D96-93D5-97BC3225E975"), CategoryName = "Category 1" } },
                    CreatedDate = DateTime.Now,
                    Description = "Product 1",
                    Name = "Product 1",
                    Price = 123
                });
            dbContext.Categories.AddRange(
                new Category()
                {
                    Id = Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"),
                    CategoryName = "Category 1",
                    CreatedDate = DateTime.Now,

                });
            dbContext.SaveChanges();

            return dbContext;
        }

        [Test]
        public async Task GetProducts_WithProductFilters_ReturnsFilteredProducts()
        {
            //Arrange
            _mockIRepositoryBase
                .Setup(_ => _.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<List<Expression<Func<Product, object>>>>()))
                .Returns(dbContext.Products);
            _mockIHttpContextAccessor.Setup(_ => _.HttpContext.Response.Headers).Returns(new HeaderDictionary());
            _mockIMapper.Setup(_ => _.Map<List<ProductListDto>>(It.IsAny<List<Product>>()))
                .Returns(dbContext.Products.Select(product => new ProductListDto()
                {
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Id = product.Id,
                    Categories = product.Categories.Select(_ => new CategoryListDto() { CategoryName = _.CategoryName, Id = _.Id }).ToList()
                }).ToList());
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);

            //Act
            var actualProducts = productService.GetProducts(new Core.Filters.Product.ProductRequestFilter() { Page = 0, Size = 5 }).ResponseValue;

            //Assert
            actualProducts.Should().BeEquivalentTo(dbContext.Products.Select(_ => new ProductListDto()
            {
                Price = _.Price,
                Categories = _.Categories.Select(_ => new CategoryListDto() { CategoryName = _.CategoryName, Id = _.Id }).ToList(),
                Description = _.Description,
                Name = _.Name,
                Id = _.Id,
            }));
        }

        [Test]
        public void GetProducts_WithInvalidPageAndSizeFilters_ReturnsFilteredProducts()
        {
            //Arrange
            _mockIRepositoryBase
                .Setup(_ => _.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<List<Expression<Func<Product, object>>>>()))
                .Returns(dbContext.Products);
            _mockIHttpContextAccessor.Setup(_ => _.HttpContext.Response.Headers).Returns(new HeaderDictionary());
            _mockIMapper.Setup(_ => _.Map<List<ProductListDto>>(It.IsAny<List<Product>>()))
                .Returns(dbContext.Products.Select(product => new ProductListDto()
                {
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Id = product.Id,
                    Categories = product.Categories.Select(_ => new CategoryListDto() { CategoryName = _.CategoryName, Id = _.Id }).ToList()
                }).ToList());
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);

            //Act
            var actualProducts = productService.GetProducts(new Core.Filters.Product.ProductRequestFilter() { Page = -2, Size = -2 }).ResponseValue;

            //Assert
            actualProducts.Should().BeEquivalentTo(dbContext.Products.Select(_ => new ProductListDto()
            {
                Price = _.Price,
                Categories = _.Categories.Select(_ => new CategoryListDto() { CategoryName = _.CategoryName, Id = _.Id }).ToList(),
                Description = _.Description,
                Name = _.Name,
                Id = _.Id,
            }));
        }

        [Test]
        public async Task GetProductById_WithProductId_ReturnsProductWithGivenId()
        {
            //Arrange
            Guid userId = Guid.Parse("4FC4E4AC-FCF8-468E-821B-48372AF4B9D0");
            Product validProduct = dbContext.Products.FirstOrDefault(_ => _.Id == userId);
            _mockIMapper.Setup(_ => _.Map<ProductListDto>(It.IsAny<Product>()))
                .Returns(new ProductListDto()
                {
                    Description = validProduct.Description,
                    Name = validProduct.Name,
                    Price = validProduct.Price,
                    Id = validProduct.Id,
                    Categories = validProduct.Categories.Select(_ => new CategoryListDto()
                    { CategoryName = _.CategoryName, Id = _.Id }).ToList()
                });
            _mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Product, object>>>>()))
                .ReturnsAsync(validProduct);

            ProductService productService = new
                ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);

            //Act
            var responseProduct = await productService.GetProductByIdAsync(validProduct.Id.ToString());

            ////Assert  
            validProduct.Should().BeEquivalentTo(responseProduct);
        }

        [Test]
        public async Task GetProductById_WithInvalidProductId_ThrowsNotFoundException()
        {
            //Arrange
            Guid userId = Guid.Parse("4FC4E4AC-FCF8-468E-821B-48372AF4B999");
            _mockIUnitOfWork.Setup(_ => _.GetRepository<Product>())
                .Returns(_mockIRepositoryBase.Object);

            //Act
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);
            Func<Task> exception = async () => await productService.GetProductByIdAsync(userId.ToString());

            //Assert
            await exception.Should().ThrowAsync<ProductNotFoundException>();
        }

        [Test]
        public async Task AddProduct_WithProductDetail_ReturnsTrue()
        {
            //Arrange
            string categoryId = "8963B9A6-30D4-481B-B421-214FCC2640FF";
            ProductAddDto productDto = new()
            {
                Description = "Product 2",
                Name = "Product 2",
                Price = 111,
                CategoryIds = new List<string>() { "ADF91C00-23F0-4D96-93D5-97BC3225E975" }
            };
            _mockIMapper.Setup(_ => _.Map<Product>(It.IsAny<ProductAddDto>())).Returns(new Product()
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
            });
            _mockIUnitOfWork
                .Setup(_ => _.GetRepository<Category>().GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Category, object>>>>()))
                .ReturnsAsync(dbContext.Categories.First(_ => _.Id == Guid.Parse(categoryId)));

            //Act
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);
            bool result = await productService.AddProductAsync(productDto);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveProduct_WithProductId_ReturnsTrue()
        {
            //Arrange
            const string productId = "4FC4E4AC-FCF8-468E-821B-48372AF4B9D0";
            _mockIHttpContextAccessor.Setup(_ => _.HttpContext.User).Returns(new System.Security.Claims.ClaimsPrincipal());
            _mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Product, object>>>>()))
                .ReturnsAsync(dbContext.Products.First(_ => _.Id == Guid.Parse(productId)));
            _mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new Product());
            //_mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Product, object>>>>()))
            //   .ReturnsAsync(dbContext.Products.First(_ => _.Id == Guid.Parse(productId)));
            _mockIRepositoryBase.Setup(_ => _.SafeRemove(It.IsAny<Product>()));

            //Act
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);
            await productService.RemoveProductAsync(productId);
            var products = dbContext.Products;
            //Assert
        }

        [Test]
        public async Task RemoveProduct_WithInvalidProductId_ThrowsBadRequestException()
        {
            //Arrange
            _mockIUnitOfWork.Setup(_ => _.GetRepository<Product>()).Returns(_mockIRepositoryBase.Object);
            _mockIHttpContextAccessor.Setup(_ => _.HttpContext.User).Returns(new System.Security.Claims.ClaimsPrincipal());
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);

            //Act
            Func<Task> result = async () =>
            {
                await productService.RemoveProductAsync("");
            };

            //Assert
            await result.Should().ThrowAsync<ProductNotFoundException>();
        }

        [Test]
        public async Task UpdateProduct_WithGivenId_ReturnsTrue()
        {
            _mockIMapper.Setup(_ => _.Map(It.IsAny<ProductUpdateDto>, It.IsAny<Product>()));

        }

        [Test]
        public async Task UpdateProduct_WithInvalidId_ThrowsBadRequestException()
        {
            //Arrange
            _mockIUnitOfWork.Setup(_ => _.GetRepository<Product>()).Returns(_mockIRepositoryBase.Object);
            _mockIHttpContextAccessor.Setup(_ => _.HttpContext.User).Returns(new System.Security.Claims.ClaimsPrincipal());
            ProductService productService = new ProductService(_mockIUnitOfWork.Object, _mockIMapper.Object, _mockIHttpContextAccessor.Object);

            //Act
            Func<Task> result = async () =>
            {
                await productService.UpdateProductAsync(new ProductUpdateDto());
            };

            //Assert
            await result.Should().ThrowAsync<ProductNotFoundException>();
        }
    }
}