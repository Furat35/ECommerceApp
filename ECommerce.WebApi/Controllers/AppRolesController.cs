using ECommerce.Core.ActionFilters;
using ECommerce.Core.DTOs.AppRole;
using ECommerce.Core.Filters.AppRole;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Admin", Roles = $"{RoleConsts.Admin}")]
    public class AppRolesController : ControllerBase
    {
        private readonly IAppRoleService _RoleService;

        public AppRolesController(IAppRoleService RoleService)
        {
            _RoleService = RoleService;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet]
        [AddCache]
        public IActionResult GetAllRoles([FromQuery] AppRoleRequestFilter filters)
        {
            AppRoleResponseFilter<List<AppRoleListDto>> response = _RoleService.GetRoles(filters);
            return Ok(response.ResponseValue);
        }

        /// <summary>
        /// Get the role with the given id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllRoles([FromRoute] string id)
        {
            AppRoleListDto Role = await _RoleService.GetRoleByIdAsync(id);
            return Ok(Role);
        }

        /// <summary>
        /// Add a role
        /// </summary>
        [HttpPost]
        [RemoveCache(CacheActionName = nameof(GetAllRoles))]
        public async Task<IActionResult> AddRole([FromBody] AppRoleAddDto RoleDto)
        {
            await _RoleService.CreateRoleAsync(RoleDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Update a role
        /// </summary>
        [HttpPut]
        [RemoveCache(CacheActionName = nameof(GetAllRoles))]
        public async Task<IActionResult> UpdateRole(AppRoleUpdateDto RoleDto)
        {
            await _RoleService.UpdateRoleAsync(RoleDto);
            return Ok();
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        [HttpDelete]
        [RemoveCache(CacheActionName = nameof(GetAllRoles))]
        public async Task<IActionResult> DeleteRole([FromQuery] string id)
        {
            await _RoleService.RemoveRoleAsync(id);
            return Ok();
        }
    }
}
