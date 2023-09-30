using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Helpers;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(AppUserLoginDto loginDto)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("GetAllCategories", "Categories");
            JwtResponse response = await _authenticationService.LoginAsync(loginDto);
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(AppUserAddDto registerDto)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("GetAllCategories", "Categories");
            await _authenticationService.RegisterAsync(registerDto);
            return Ok();
        }
    }
}
