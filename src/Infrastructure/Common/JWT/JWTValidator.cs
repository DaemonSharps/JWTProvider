using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Common.JWT
{
    public class JWTValidator
    {
        private readonly SymmetricSecurityKey _secretKey;
        private readonly string _issuer;
        private JwtSecurityToken _token;

        public JWTValidator(string secretKey, string issuer)
        {
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            _issuer = issuer;
        }

        public JwtPayload TokenPayload => _token?.Payload;

        public JwtSecurityToken Token => _token;

        public static JWTValidator GetValidator(string secretKey, string issuer)
            => new(secretKey, issuer);

        public JWTValidator ValidateRefreshToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {

                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _secretKey
            };

            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken) _token = securityToken as JwtSecurityToken;
            else throw new SecurityTokenValidationException("Invalid token");

            return this;
        }
    }
}
