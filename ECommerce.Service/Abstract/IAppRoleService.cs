using ECommerce.Core.DTOs.AppRole;
using ECommerce.Core.Filters.AppRole;

namespace ECommerce.Service.Abstract
{
    public interface IAppRoleService
    {
        AppRoleResponseFilter<List<AppRoleListDto>> GetRoles(AppRoleRequestFilter filters);
        Task<AppRoleListDto> GetRoleByIdAsync(string id);
        Task CreateRoleAsync(AppRoleAddDto roleDto);
        Task<bool> UpdateRoleAsync(AppRoleUpdateDto roleDto);
        Task RemoveRoleAsync(string id);
    }
}
