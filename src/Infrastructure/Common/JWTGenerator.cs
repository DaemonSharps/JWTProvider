using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Infrastructure.Middleware;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Common
{
    /// <summary>
    /// Генератор JWT
    /// </summary>
    public class JWTGenerator
    {
        private readonly SecurityKey _accessKey;
        private readonly string _issuer;
        private string _acessToken;

        /// <summary>
        /// Стандартное время жизни Access Token
        /// </summary>
        public static TimeSpan ExpiresDefault => TimeSpan.FromMinutes(5);

        public string AcessToken => _acessToken;

        public JWTGenerator(string accessKey, string issuer)
        {
            _accessKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessKey));
            _issuer = issuer;
        }

        /// <summary>
        /// Создать экземпляр генератора
        /// </summary>
        public static JWTGenerator GetGenerator(string accessKey, string refreshKey, string issuer)
            => new(accessKey, issuer);

        /// <summary>
        /// Создать экземпляр генератора из класса конфигурации
        /// </summary>
        public static JWTGenerator GetGenerator(TokenOptions options)
            => new(options?.AccessKey, options?.Issuer);

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
                new (JWTClaimKeys.Email, user.Email)
            };
            if (!string.IsNullOrEmpty(user.MiddleName)) claims.Add(new(JWTClaimKeys.MiddleName, user.MiddleName));
            if (!string.IsNullOrEmpty(user.LastName)) claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));
            if (!string.IsNullOrEmpty(user.FirstName)) claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));

            _acessToken = MakeStringToken(_accessKey, expiresAfter ?? ExpiresDefault, claims.ToArray());

            return this;
        }

        /// <summary>
        /// Создать строковое представление JWT
        /// </summary>
        /// <param name="expiresAfter">Время жизни токена</param>
        /// <param name="claims">Список полей в payload</param>
        /// <returns></returns>
        private string MakeStringToken(SecurityKey secretKey, TimeSpan expiresAfter, params Claim[] claims)
        {
            var ceredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);
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
