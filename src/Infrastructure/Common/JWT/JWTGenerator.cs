using Infrastructure.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Common.JWT
{
    /// <summary>
    /// Генератор JWT
    /// </summary>
    public class JWTGenerator
    {
        private readonly SecurityKey _secretKey;
        private readonly string _issuer;
        private readonly TimeSpan _expiresDefault = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _refreshExpiresDefault = TimeSpan.FromDays(7);
        private string _acessToken;
        private string _refreshToken;

        public string AcessToken => _acessToken;
        public string RefteshToken => _refreshToken;

        public JWTGenerator(string secretKey, string issuer)
        {
            _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            _issuer = issuer;
        }

        /// <summary>
        /// Создать экземпляр генератора
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static JWTGenerator GetGenerator(string secretKey, string issuer)
            => new(secretKey, issuer);

        /// <summary>
        /// Создать JWT из модели пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="expiresAfter">Время жизни токена</param>
        /// <returns></returns>
        public JWTGenerator CreateAcessToken(User user, TimeSpan? expiresAfter = null)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Email, user.Email),
                new (JwtRegisteredClaimNames.GivenName, user.FirstName),
                new (JWTClaimKeys.MiddleName, user.MiddleName),
                new (JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            if (user is DataBase.User userDB)
            {
                claims.Add(new(JWTClaimKeys.Role, userDB.Role?.Name));
                claims.Add(new(JWTClaimKeys.Login, userDB.Login?.GetFullLogin()));
            }

            _acessToken = MakeStringToken(expiresAfter ?? _expiresDefault, claims.ToArray());

            return this;
        }

        /// <summary>
        /// Создать JWT из электронной почты
        /// </summary>
        /// <param name="email">Почта</param>
        /// <param name="expiresAfter">Время жизни токена</param>
        /// <returns></returns>
        public JWTGenerator CreateRefreshToken(string email, TimeSpan? expiresAfter = null)
        {
            var claim = new Claim(JwtRegisteredClaimNames.Email, email);
            _refreshToken = MakeStringToken(expiresAfter ?? _expiresDefault, claim);

            return this;
        }

        public JWTGenerator CreateTokenPair(User user)
        {
            CreateAcessToken(user);
            CreateRefreshToken(user.Email, _refreshExpiresDefault);
            return this;
        }

        /// <summary>
        /// Создать строковое представление JWT
        /// </summary>
        /// <param name="expiresAfter">Время жизни токена</param>
        /// <param name="claims">Список полей в payload</param>
        /// <returns></returns>
        private string MakeStringToken(TimeSpan expiresAfter, params Claim[] claims)
        {
            var ceredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha512Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMilliseconds(expiresAfter.TotalMilliseconds),
                SigningCredentials = ceredentials,
                Issuer = _issuer
            };
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }
    }
}
