using System.Security.Claims;
using Infrastructure.Constants;

namespace Infrastructure.Extentions
{
    public static class ClaimsExtentions
    {
        public static string GetEmail(this ClaimsPrincipal claims)
            => claims.FindFirstValue(JWTClaimKeys.Email);
    }
}
