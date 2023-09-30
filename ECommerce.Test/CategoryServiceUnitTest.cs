using AutoMapper;
using ECommerce.Core.DTOs.Category;
using ECommerce.Core.Exceptions.Category;
using ECommerce.Core.Filters.Category;
using ECommerce.Data.Repositories;
using ECommerce.Data.Repositories.EntityFrameworkContext;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities;
using ECommerce.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Linq.Expressions;

namespace ECommerce.Test
{
    public class CategoryServiceUnitTest
    {
        Mock<IMapper> _mockIMapper = new Mock<IMapper>();
        Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();
        Mock<IHttpContextAccessor> _mockContextAccessor = new Mock<IHttpContextAccessor>();
        Mock<IRepositoryBase<Category>> _mockIRepositoryBase = new Mock<IRepositoryBase<Category>>();
        EfContext _dbContext;
        public CategoryServiceUnitTest()
        {
            _dbContext = CreateMemoryDbContext();
            _mockUnitOfWork.Setup(_ => _.GetRepository<Category>()).Returns(_mockIRepositoryBase.Object);
        }

        private EfContext CreateMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options;
            var dbContext = new EfContext(options);
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
        public async Task GetCategoryById_WithCategoryId_ReturnsCategoryWithGivenId()
        {
            //Arrange
            Guid categoryId = Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF");
            Category expectedCategory = _dbContext.Categories.Find(categoryId);
            _mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Category, object>>>>()))
                .ReturnsAsync(expectedCategory);
            _mockIMapper.Setup(_ => _.Map<CategoryListDto>(It.IsAny<Category>())).Returns(new CategoryListDto()
            {
                CategoryName = expectedCategory.CategoryName,
                Id = expectedCategory.Id
            });
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            CategoryListDto actualCategory = await categoryService.GetCategoryByIdAsync(categoryId.ToString());

            //Assert
            actualCategory.Should().BeEquivalentTo(new CategoryListDto()
            {
                CategoryName = expectedCategory.CategoryName,
                Id = expectedCategory.Id
            });
        }

        [Test]
        public async Task GetCategoryById_WithInvalidCategoryId_ThrowsCategoryNotFound()
        {
            //Arrange 
            const string categoryId = "8963B9A6-30D4-481B-B421-214FCC2640F5F";
            _mockUnitOfWork.Setup(_ => _.GetRepository<Category>())
                .Returns(_mockIRepositoryBase.Object);
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            Func<Task> expectedException = async () =>
            {
                await categoryService.GetCategoryByIdAsync(categoryId);
            };

            //Assert
            await expectedException.Should().ThrowAsync<CategoryNotFoundException>();
        }

        [Test]
        public async Task GetCategories_WithCategoryFilters_ReturnsAllCategories()
        {
            //Arrange
            _mockIRepositoryBase
                .Setup(_ => _.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<List<Expression<Func<Category, object>>>>()))
                .Returns(_dbContext.Categories);
            _mockContextAccessor
                .Setup(_ => _.HttpContext.Response.Headers.Add(It.IsAny<string>(), It.IsAny<StringValues>()));
            _mockIMapper
                .Setup(_ => _.Map<List<CategoryListDto>>(It.IsAny<List<Category>>()))
                .Returns(_dbContext.Categories.Select(_ => new CategoryListDto()
                {
                    CategoryName = _.CategoryName,
                    Id = _.Id
                }).ToList());
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            var categories = categoryService.GetCategories(new CategoryRequestFilter() { Page = 0, Size = 5 });

            //Assert 
            categories.ResponseValue.Should().BeEquivalentTo(_dbContext.Categories.Select(_ => new CategoryListDto()
            {
                CategoryName = _.CategoryName,
                Id = _.Id
            }));
        }

        [Test]
        public async Task UpdateCategory_WithValidIdInDTO_ReturnsModifiedCategory()
        {
            //Arrange            
            Category category = _dbContext.Categories.Find(Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"));
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"),
                CategoryName = "Deneme"
            };
            _mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Category, object>>>>()))
                .ReturnsAsync(category);
            _mockIMapper.Setup(_ => _.Map(It.IsAny<CategoryUpdateDto>(), It.IsAny<Category>())).Returns(category);
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            bool result = await categoryService.UpdateCategoryAsync(categoryUpdateDto);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateCategory_WithInvalidIdInDTO_ThrowsCategoryNotFoundException()
        {
            //Arrange            
            Category category = _dbContext.Categories.Find(Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640F1"));
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640F1"),
                CategoryName = "Deneme"
            };
            //_mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Category, object>>>>()));
            _mockIMapper.Setup(_ => _.Map(It.IsAny<CategoryUpdateDto>(), It.IsAny<Category>())).Returns(category);
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            var result = async () =>
            {
                await categoryService.UpdateCategoryAsync(categoryUpdateDto);
            };

            //Assert
            await result.Should().ThrowAsync<CategoryNotFoundException>();
        }

        [Test]
        public async Task UpdateCategory_WithAlreadyUsedCategoryName_ThrowsCategoryAlreadyExistsException()
        {
            //Arrange            
            Category category = _dbContext.Categories.Find(Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"));
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = Guid.Parse("1163B9A6-30D4-481B-B421-214FCC264066"),
                CategoryName = "Category 1",
            };
            _mockIRepositoryBase.Setup(_ => _.GetFirstWhereAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(category);
            _mockIRepositoryBase.Setup(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<List<Expression<Func<Category, object>>>>()))
                .ReturnsAsync(category);
            _mockIMapper.Setup(_ => _.Map(It.IsAny<CategoryUpdateDto>(), It.IsAny<Category>()))
                .Returns(category);
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            var result = async () =>
            {
                await categoryService.UpdateCategoryAsync(categoryUpdateDto);
            };

            //Assert
            await result.Should().ThrowAsync<CategoryAlreadyExistsException>();
        }

        [Test]
        public async Task AddCategory_WithCategoryDetail_ReturnsTrue()
        {
            //Arrange
            CategoryAddDto categorAddDto = new CategoryAddDto()
            {
                CategoryName = "Category 1",
            };
            _mockIMapper.Setup(_ => _.Map<Category>(It.IsAny<CategoryAddDto>())).Returns(new Category
            {
                CategoryName = categorAddDto.CategoryName,
            });
            _mockIRepositoryBase.Setup(_ => _.AddAsync(It.IsAny<Category>()));
            _mockUnitOfWork.Setup(_ => _.GetRepository<Category>()).Returns(_mockIRepositoryBase.Object);
            CategoryService categoryService = new CategoryService(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            bool result = await categoryService.AddCategoryAsync(categorAddDto);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveCategory_WithInvalidInId_ReturnsTrue()
        {
            //Arrange
            CategoryService categoryService = new(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            var result = async () => await categoryService.RemoveCategoryAsync("");

            //Assert
            result.Should().ThrowAsync<CategoryNotFoundException>();
        }

        [Test]
        public async Task RemoveCategory_WithGivenIdInDTO_ReturnsTrue()
        {
            //Arrange
            CategoryService categoryService = new(_mockUnitOfWork.Object, _mockIMapper.Object, _mockContextAccessor.Object);

            //Act
            var result = async () => await categoryService.RemoveCategoryAsync("");

            //Assert
            result.Should().ThrowAsync<CategoryNotFoundException>();
        }
    }
}
