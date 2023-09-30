using System.Security.Claims;

namespace ECommerce.Service.Extensions
{
    public static class GetActiveUser
    {
        public static string GetUserName(this ClaimsPrincipal claims)
        {
            ClaimsIdentity userPrincipals = claims.Identities.FirstOrDefault(_ => _.IsAuthenticated);
            return userPrincipals?.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static string GetUserId(this ClaimsPrincipal claims)
        {
            ClaimsIdentity userPrincipals = claims.Identities.FirstOrDefault(_ => _.IsAuthenticated);
            return userPrincipals?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal claims)
        {
            ClaimsIdentity userPrincipals = claims.Identities.FirstOrDefault(_ => _.IsAuthenticated);
            return userPrincipals?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
