using AutoMapper;
using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Exceptions.AppUser;
using ECommerce.Core.Filters.AppUser;
using ECommerce.Core.Helpers;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using ECommerce.Service.Abstract;
using ECommerce.Service.Extensions;
using ECommerce.Service.Helpers.FilterServices;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Service.Concrete
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;
        private readonly IUnitOfWork _unitOfWork;

        public AppUserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, IHttpContextAccessor httpContext, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _httpContext = httpContext.HttpContext;
            _unitOfWork = unitOfWork;
        }

        public AppUserResponseFilter<List<AppUserListDto>> GetUsers(AppUserRequestFilter filters)
        {
            IQueryable<AppUser> users = _userManager.Users.Where(_ => _.DeletedDate == null);
            AppUserResponseFilter<List<AppUserListDto>> response = new AppUserFilterService(_mapper).FilterUsers(users, filters);
            new AddHeadersToResponse(_httpContext).AddToResponse(response.Headers);

            return response;
        }

        public async Task<AppUserListDto> GetUserByIdAsync(string id)
        {
            AppUser user = await GetUserById(id);
            return _mapper.Map<AppUserListDto>(user);
        }

        public async Task CreateUserAsync(AppUserAddDto userDto)
        {
            AppUser user = _mapper.Map<AppUser>(userDto);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                IdentityResult identityResult = await _userManager.CreateAsync(user, userDto.Password);
                if (!identityResult.Succeeded)
                    throw new Exception(identityResult.Errors.First().Description);
                await AddRolesToUserAsync(user, userDto.RoleIds);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new AppUserBadRequestException(ex.Message);
            }

            await _unitOfWork.CommitTransactionAsync();
        }

        public async Task UpdateUserAsync(AppUserUpdateDto userDto)
        {
            AppUser user = await GetUserById(userDto.Id.ToString());
            user = _mapper.Map(userDto, user);
            user.ModifiedDate = DateTime.Now;
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await IdentityResultAsync(user);
                await AddRolesToUserAsync(user, userDto.RoleIds);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new AppUserInternalServerException();
            }

            await _unitOfWork.CommitTransactionAsync();
        }

        public async Task RemoveUserAsync(string id)
        {
            AppUser user = await GetUserById(id);
            user.DeletedDate = DateTime.Now;
            user.DeletedBy = _httpContext.User.GetUserName();
            await IdentityResultAsync(user);
        }

        private async Task AddRolesToUserAsync(AppUser user, List<string> roleIds)
        {
            if (!roleIds.Any())
                throw new Exception("Role is not assigned!");

            if (user.Id != null)
            {
                IdentityResult identityResult = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                if (!identityResult.Succeeded)
                    throw new Exception(identityResult.Errors.First().Description);
            }

            bool hasAtLeastOneValidRole = false;
            foreach (var roleId in roleIds)
            {
                AppRole roleExists = await _roleManager.FindByIdAsync(roleId);
                if (roleExists != null)
                    try
                    {
                        IdentityResult addIdentityResult = await _userManager.AddToRoleAsync(user, roleExists.Name);
                        if (addIdentityResult.Succeeded)
                            hasAtLeastOneValidRole = true;
                        else
                            throw new Exception(addIdentityResult.Errors.First().Description);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
            }

            if (!hasAtLeastOneValidRole)
                throw new Exception("Invalid role/s");
        }

        private async Task<AppUser> GetUserById(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user is null)
                throw new AppUserNotFoundException();

            return user;
        }

        private async Task IdentityResultAsync(AppUser user)
        {
            IdentityResult identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
                throw new AppUserInternalServerException();
        }
    }
}
