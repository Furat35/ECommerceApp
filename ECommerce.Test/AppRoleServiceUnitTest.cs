using AutoMapper;
using ECommerce.Core.DTOs.AppRole;
using ECommerce.Core.Exceptions.AppRole;
using ECommerce.Data.Repositories.EntityFrameworkContext;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using ECommerce.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerce.Test
{
    public class AppRoleServiceUnitTest
    {
        private readonly Mock<RoleManager<AppRole>> _mockRoleManager = new Mock<RoleManager<AppRole>>(Mock.Of<IRoleStore<AppRole>>(), null, null, null, null);
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IHttpContextAccessor> _mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
        private readonly EfContext _dbContext;
        public AppRoleServiceUnitTest()
        {
            _dbContext = CreateMemoryDbContext();
        }

        private EfContext CreateMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options;
            var dbContext = new EfContext(options);
            dbContext.Roles.AddRange(
                new AppRole()
                {
                    Id = Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
            dbContext.SaveChanges();
            return dbContext;
        }

        [Test]
        public async Task GetRoleById_WithRoleId_ReturnsRoleWithGivenId()
        {
            //Arrange
            const string roleId = "8963B9A6-30D4-481B-B421-214FCC2640FF";
            AppRole role = _dbContext.Find<AppRole>(Guid.Parse(roleId));
            _mockRoleManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);
            _mockMapper.Setup(_ => _.Map<AppRoleListDto>(It.IsAny<AppRole>())).Returns(new AppRoleListDto
            {
                Id = roleId,
                Name = role.Name
            });
            AppRoleService roleService = new AppRoleService(_mockMapper.Object, _mockRoleManager.Object, _mockIHttpContextAccessor.Object);

            //Act
            var actualRole = await roleService.GetRoleByIdAsync(roleId);

            //Assert
            actualRole.Should().BeEquivalentTo(new AppRoleListDto
            {
                Id = roleId,
                Name = role.Name
            });
        }

        [Test]
        public async Task GetRoleById_WithInvalidRoleId_ThrowsException()
        {
            //Arrange
            const string roleId = "8963B9A6-30D4-481B-B421-214FCC2640F2";
            AppRoleService roleService = new AppRoleService(_mockMapper.Object, _mockRoleManager.Object, _mockIHttpContextAccessor.Object);

            //Act

            var actualRole = async () =>
            {
                return await roleService.GetRoleByIdAsync(roleId);
            };

            //Assert
            await actualRole.Should().ThrowAsync<AppRoleNotFoundException>();
        }

        [Test]
        public async Task UpdateRole_WithInvalidRoleIdInDTO_ReturnsVoid()
        {
            //Arrange
            AppRoleUpdateDto role = new()
            {
                Id = "1163B9A6-30D4-481B-B421-214FCC2640F4",
                Name = "Admin",
            };
            AppRoleService roleService = new(_mockMapper.Object, _mockRoleManager.Object, _mockIHttpContextAccessor.Object);

            //Act
            var result = async () => await roleService.UpdateRoleAsync(role);

            //Assert
            await result.Should().ThrowAsync<AppRoleNotFoundException>();
        }

        [Test]
        public async Task UpdateRole_WithInvalidRoleName_ThrowsAppRoleAlreadyExistsException()
        {
            //Arrange
            AppRole role = _dbContext.Roles.Find(Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"));
            AppRoleUpdateDto roleDto = new()
            {
                Id = "1163B9A6-30D4-481B-B421-214FCC2640F4",
                Name = "Admin",
            };
            _mockRoleManager.Setup(_ => _.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(role);
            AppRoleService roleService = new(_mockMapper.Object, _mockRoleManager.Object, _mockIHttpContextAccessor.Object);

            //Act
            var result = async () => await roleService.UpdateRoleAsync(roleDto);

            //Assert
            await result.Should().ThrowAsync<AppRoleAlreadyExistsException>();
        }

        [Test]
        public async Task UpdateRole_DeletedRoleIsRestored_ReturnsFalse()
        {
            AppRole role = _dbContext.Roles.Find(Guid.Parse("8963B9A6-30D4-481B-B421-214FCC2640FF"));
            role.DeletedDate = DateTime.Now;
            _dbContext.SaveChanges();
            AppRoleUpdateDto roleDto = new()
            {
                Id = "8963B9A6-30D4-481B-B421-214FCC2640FF",
                Name = "Admin",
            };
            _mockRoleManager.Setup(_ => _.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(role);
            AppRoleService roleService = new(_mockMapper.Object, _mockRoleManager.Object, _mockIHttpContextAccessor.Object);

            //Act
            bool result = await roleService.UpdateRoleAsync(roleDto);

            //Assert
            Assert.IsTrue(!result);
        }

        [Test]
        public async Task RemoveRole_WithInvalidRoleId_ThrowsAppRoleNotFoundException()
        {
            //Arrange
            const string roleId = "8963B9A6-30D4-481B-B421-214FCC2640FF";
            //AppRole role = _dbContext.Find<AppRole>(Guid.Parse(roleId));
            //_mockRoleManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);
            //_mockMapper.Setup(_ => _.Map<AppRoleListDto>(It.IsAny<AppRole>())).Returns(new AppRoleListDto
            //{
            //    Id = roleId,
            //    Name = role.Name
            //});
            AppRoleService roleService = new AppRoleService(_mockMapper.Object, _mockRoleManager.Object, _mockIHttpContextAccessor.Object);

            //Act
            var result = async () => await roleService.RemoveRoleAsync(roleId);

            //Assert
            await result.Should().ThrowAsync<AppRoleNotFoundException>();
        }
    }
}
