using AutoMapper;
using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Exceptions.AppUser;
using ECommerce.Data.Repositories.EntityFrameworkContext;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using ECommerce.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerce.Test
{
    public class AppUserServiceUnitTest
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
        private readonly Mock<RoleManager<AppRole>> _mockRoleManager = new(Mock.Of<IRoleStore<AppRole>>(), null, null, null, null);
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private readonly EfContext _dbContext;
        public AppUserServiceUnitTest()
        {
            _dbContext = CreateMemoryDbContext();
        }

        private EfContext CreateMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options;
            var dbContext = new EfContext(options);
            dbContext.Users.AddRange(
                new AppUser()
                {
                    Id = Guid.Parse("1163B9A6-30D4-481B-B421-214FCC264011"),
                    UserName = "Firat",
                    Email = "furat@hotmail.com",
                    PhoneNumber = "1234567890",
                });
            dbContext.SaveChanges();
            return dbContext;
        }

        [Test]
        public async Task GetUserById_WithValidUserId_ReturnsUserWithGivenId()
        {
            //Arrange
            AppUser user = _dbContext.Users.Find(Guid.Parse("1163B9A6-30D4-481B-B421-214FCC264011"));
            AppUserListDto userDto = new()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
            _mockUserManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(_ => _.Map<AppUserListDto>(It.IsAny<AppUser>()))
                .Returns(userDto);
            AppUserService userService = new(_mockUserManager.Object, _mockRoleManager.Object, _mockMapper.Object, _mockHttpContextAccessor.Object, _mockUnitOfWork.Object);

            //Act
            AppUserListDto result = await userService.GetUserByIdAsync(user.Id.ToString());

            //Assert
            result.Should().BeEquivalentTo(userDto);
        }

        [Test]
        public async Task GetUserById_WithInvalidUserId_ReturnsUserWithGivenId()
        {
            //Arrange
            //AppUser user = _dbContext.Users.Find(Guid.Parse("1163B9A6-30D4-481B-B421-214FCC264022"));
            AppUserService userService = new(_mockUserManager.Object, _mockRoleManager.Object, _mockMapper.Object, _mockHttpContextAccessor.Object, _mockUnitOfWork.Object);

            //Act
            var result = async () => await userService.GetUserByIdAsync("");

            //Assert
            await result.Should().ThrowAsync<AppUserNotFoundException>();
        }

        [Test]
        public async Task UpdateUser_WithInvalidUserIdInDTO_ThrowsAppUserNotFoundException()
        {
            //Arrange
            AppUserUpdateDto userDto = new()
            {
                Id = Guid.Parse("1163B9A6-30D4-481B-B421-214FCC264011"),
                UserName = "Firat",
                Email = "furat@hotmail.com",
                PhoneNumber = "1234567890"
            };
            AppUserService service = new(_mockUserManager.Object, _mockRoleManager.Object, _mockMapper.Object, _mockHttpContextAccessor.Object, _mockUnitOfWork.Object);

            //Act
            var result = async () => await service.UpdateUserAsync(userDto);

            //Assert
            await result.Should().ThrowAsync<AppUserNotFoundException>();
        }
    }
}
