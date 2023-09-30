using AutoMapper;
using ECommerce.Core.DTOs.AppRole;
using ECommerce.Core.Exceptions.AppRole;
using ECommerce.Core.Filters.AppRole;
using ECommerce.Core.Helpers;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using ECommerce.Service.Abstract;
using ECommerce.Service.Extensions;
using ECommerce.Service.Helpers.FilterServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace ECommerce.Service.Concrete
{
    public class AppRoleService : IAppRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public AppRoleService(IMapper mapper, RoleManager<AppRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public AppRoleResponseFilter<List<AppRoleListDto>> GetRoles(AppRoleRequestFilter filters)
        {
            IQueryable<AppRole> roles = _roleManager.Roles.Where(_ => _.DeletedDate == null);
            AppRoleResponseFilter<List<AppRoleListDto>> response = new AppRoleFilterService(_mapper).FilterRoles(roles, filters);
            new AddHeadersToResponse(_httpContext).AddToResponse(response.Headers);

            return response;
        }

        public async Task<AppRoleListDto> GetRoleByIdAsync(string id)
        {
            AppRole role = await GetRoleById(id);
            return _mapper.Map<AppRoleListDto>(role);
        }

        public async Task CreateRoleAsync(AppRoleAddDto roleDto)
        {
            if (!await IsValidRoleName(roleDto.Name))
                return;

            AppRole role = _mapper.Map<AppRole>(roleDto);
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult identityResult = await _roleManager.CreateAsync(role);
            if (!identityResult.Succeeded)
                throw new AppRoleInternalServerException();
        }

        public async Task<bool> UpdateRoleAsync(AppRoleUpdateDto roleDto)
        {
            if (!await IsValidRoleName(roleDto.Name))
                return false;

            AppRole role = await GetRoleById(roleDto.Id);
            _mapper.Map(roleDto, role);
            role.ModifiedDate = DateTime.UtcNow;
            await IdentityUpdateResultAsync(role);
            return true;
        }

        public async Task RemoveRoleAsync(string id)
        {
            AppRole role = await GetRoleById(id);
            role.DeletedDate = DateTime.UtcNow;
            role.DeletedBy = _httpContext.User.GetUserName();
            await IdentityUpdateResultAsync(role);
        }

        private async Task<AppRole> GetRoleById(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                throw new AppRoleNotFoundException();

            return role;
        }

        private async Task<bool> IsValidRoleName(string roleName)
        {
            AppRole role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
                return true;

            if (role.DeletedDate is null)
                throw new AppRoleAlreadyExistsException();

            role.DeletedDate = null;
            await _roleManager.UpdateAsync(role);
            return false;
        }

        private async Task IdentityUpdateResultAsync(AppRole role)
        {
            IdentityResult identityResult = await _roleManager.UpdateAsync(role);
            if (!identityResult.Succeeded)
                throw new AppRoleInternalServerException();
        }

    }
}
