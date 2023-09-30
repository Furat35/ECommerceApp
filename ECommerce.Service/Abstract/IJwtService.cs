using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Service.Abstract
{
    public interface IJwtService
    {
        JwtSecurityToken GetToken(List<Claim> authClaims);
    }
}
