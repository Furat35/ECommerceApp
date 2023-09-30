using ECommerce.Core.ActionFilters;
using ECommerce.Core.Consts;
using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Filters.AppUser;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AppUsersController : ControllerBase
    {
        private readonly IAppUserService _appUserService;

        public AppUsersController(IAppUserService AppUserService)
        {
            _appUserService = AppUserService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [AddCache]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Moderator},{RoleConsts.Admin}")]
        public async Task<IActionResult> GetAllUsers([FromQuery] AppUserRequestFilter filters)
        {
            AppUserResponseFilter<List<AppUserListDto>> response = _appUserService.GetUsers(filters);
            return Ok(response.ResponseValue);
        }

        /// <summary>
        /// Get the user with the given id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Moderator},{RoleConsts.Admin}")]
        public async Task<IActionResult> GetAllUsers([FromRoute] string id)
        {
            AppUserListDto AppUser = await _appUserService.GetUserByIdAsync(id);
            return Ok(AppUser);
        }

        /// <summary>
        /// Add a user
        /// </summary>
        [HttpPost]
        [RemoveCache(CacheActionName = nameof(GetAllUsers))]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Moderator},{RoleConsts.Admin}")]
        public async Task<IActionResult> AddUser([FromBody] AppUserAddDto AppUserDto)
        {
            await _appUserService.CreateUserAsync(AppUserDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        [HttpPut]
        [RemoveCache(CacheActionName = nameof(GetAllUsers))]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Moderator},{RoleConsts.Admin}")]
        public async Task<IActionResult> UpdateUser([FromBody] AppUserUpdateDto AppUserDto)
        {
            await _appUserService.UpdateUserAsync(AppUserDto);
            return Ok();

        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete]
        [RemoveCache(CacheActionName = nameof(GetAllUsers))]
        [Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Admin}")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            await _appUserService.RemoveUserAsync(id);
            return Ok();
        }
    }
}
