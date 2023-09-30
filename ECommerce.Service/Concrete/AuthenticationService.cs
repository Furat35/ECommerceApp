using AutoMapper;
using ECommerce.Core.Consts;
using ECommerce.Core.DTOs.AppUser;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Exceptions.AppUser;
using ECommerce.Core.Helpers;
using ECommerce.Core.RabbitMqMessageBroker;
using ECommerce.Core.RabbitMqMessageBroker.Abstract;
using ECommerce.Data.UnitOfWorks;
using ECommerce.Entity.Entities.IdentityFrameworkEntities;
using ECommerce.Service.Abstract;
using Microsoft.AspNetCore.Identity;
using RabbitMQ.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Service.Concrete
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly IRabbitMqMessagePublisher _messagePublisherService;

        public AuthenticationService(UserManager<AppUser> userManager, RoleManager<AppRole> appRole, SignInManager<AppUser> signInManager,
            IJwtService jwtService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        static AuthenticationService()
        {
            _messagePublisherService = new RabbitMqMessagePublisher();
            _messagePublisherService.Connect("E.Commerce.Users.Registered", "E_Commerce_Users", ExchangeType.Direct, "E.Commerce.Users.Registered.Key");
        }

        public async Task<JwtResponse> LoginAsync(AppUserLoginDto userDto)
        {
            AppUser user = await _userManager.FindByNameAsync(userDto.UserName);
            if (user is null)
                InvalidUserNameOrPassword();
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, userDto.Password, userDto.RememberMe, false);
            if (!signInResult.Succeeded)
                InvalidUserNameOrPassword();
            List<Claim> authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            JwtSecurityToken jwtToken = _jwtService.GetToken(authClaims);

            return new JwtResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Expiration = jwtToken.ValidTo
            };
        }

        public async Task<bool> RegisterAsync(AppUserAddDto userDto)
        {
            if (await _userManager.FindByNameAsync(userDto.UserName) != null)
                throw new BadRequestException("User already exists!");

            AppUser user = _mapper.Map<AppUser>(userDto);
            await _unitOfWork.BeginTransactionAsync();
            IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, RoleConsts.User);
                if (!roleResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }
                else
                {
                    await _unitOfWork.CommitTransactionAsync();
                    _messagePublisherService.PublishMessage($"{user.UserName} welcome to out website!");
                    return true;
                }
            }
            else
                throw new BadRequestException(result.Errors.First().Description);
        }

        private void InvalidUserNameOrPassword()
            => throw new AppUserBadRequestException("Invalid username or password");
    }
}
