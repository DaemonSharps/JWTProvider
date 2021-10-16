using System.Collections.Generic;
using System.Security.Claims;
using Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Common.JWT
{
    public class JWTGenerator
    {
        private readonly SecurityKey _secretKey;
        private readonly TimeSpan _expiresDefault = TimeSpan.FromMinutes(5);

        public JWTGenerator(string secretKey)
        {
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public static JWTGenerator GetGenerator(string secretKey)
            => new (secretKey);

        public string CreateToken(User user, TimeSpan? expiresAfter = null)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Email, user.EMail),
                new (JwtRegisteredClaimNames.GivenName, user.FirstName),
                new (JWTClaimKeys.MiddleName, user.MiddleName),
                new (JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            if (user is DataBase.User userDB)
            {
                claims.Add(new(JWTClaimKeys.Role, userDB.Role?.Name));
                claims.Add(new(JWTClaimKeys.Login, userDB.Login?.GetFullLogin()));
            }

            return MakeStringToken(expiresAfter ?? _expiresDefault, claims.ToArray());
        }

        public string CreateToken([EmailAddress] string email, TimeSpan? expiresAfter = null)
        {
            var claim = new Claim(JwtRegisteredClaimNames.Email, email);
            return MakeStringToken(expiresAfter ?? _expiresDefault, claim);
        }

        private string MakeStringToken(TimeSpan expiresAfter, params Claim[] claims)
        {
            var ceredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha512Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMilliseconds(expiresAfter.TotalMilliseconds),
                SigningCredentials = ceredentials
            };
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
