using Infrastructure.Common.JWT;
using System.Security.Claims;

namespace Infrastructure.Extentions
{
    public static class ClaimsExtentions
    {
        public static string GetEmail(this ClaimsPrincipal claims)
            => claims.FindFirstValue(JWTClaimKeys.Email);
    }
}
