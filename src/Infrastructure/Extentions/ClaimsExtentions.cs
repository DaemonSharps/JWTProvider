using Infrastructure.Common.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extentions
{
    public static class ClaimsExtentions
    {
        public static string GetEmail(this ClaimsPrincipal claims)
            => claims.FindFirstValue(JWTClaimKeys.Email);
    }
}
