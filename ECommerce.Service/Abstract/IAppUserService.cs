using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Filters.AppUser;

namespace ECommerce.Service.Abstract
{
    public interface IAppUserService
    {
        AppUserResponseFilter<List<AppUserListDto>> GetUsers(AppUserRequestFilter filters);
        Task<AppUserListDto> GetUserByIdAsync(string id);
        Task CreateUserAsync(AppUserAddDto UserDto);
        Task UpdateUserAsync(AppUserUpdateDto UserDto);
        Task RemoveUserAsync(string id);
    }
}

