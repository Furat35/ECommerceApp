using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Helpers;

namespace ECommerce.Service.Abstract
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterAsync(AppUserAddDto userDto);
        Task<JwtResponse> LoginAsync(AppUserLoginDto userDto);
        //Task<bool> ResetPassword(AppUserAddDto userDto);
    }
}
