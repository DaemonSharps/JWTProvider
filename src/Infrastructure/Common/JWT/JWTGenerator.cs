using System.Collections.Generic;
using System.Security.Claims;
using Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace Infrastructure.Common.JWT
{
    public class JWTGenerator
    {
        private readonly SecurityKey _secretKey;

        public JWTGenerator(string secretKey)
        {
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string CreateToken(User user)
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

            var ceredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha512Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = ceredentials
            };
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
