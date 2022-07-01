using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Extentions
{
    public static class ClaimsExtentions
    {
        public static string GetEmail(this ClaimsPrincipal claims)
            => claims.FindFirstValue(JwtRegisteredClaimNames.Email);
    }
}
