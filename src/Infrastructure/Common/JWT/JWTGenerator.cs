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
        private readonly SecurityKey _accessKey;
        private readonly SecurityKey _refreshKey;
        private readonly string _issuer;
        private string _acessToken;
        private string _refreshToken;


        /// <summary>
        /// Стандартное время жизни Access Token
        /// </summary>
        public static TimeSpan ExpiresDefault => TimeSpan.FromMinutes(5);

        /// <summary>
        /// Стандартное время жизни Refresh Token
        /// </summary>
        public static TimeSpan RefreshExpiresDefault => TimeSpan.FromDays(7);

        public string AcessToken => _acessToken;
        public string RefteshToken => _refreshToken;

        public JWTGenerator(string accessKey, string refreshKey, string issuer)
        {
            _accessKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessKey));
            _refreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshKey));
            _issuer = issuer;
        }

        /// <summary>
        /// Создать экземпляр генератора
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static JWTGenerator GetGenerator(string accessKey, string refreshKey, string issuer)
            => new(accessKey, refreshKey, issuer);

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

            if (user is DataBase.User userDB)
            {
                if (userDB.Role != null) claims.Add(new(JWTClaimKeys.Role, userDB.Role.Name));
                if (userDB.Login != null) claims.Add(new(JWTClaimKeys.Login, userDB.Login.GetFullLogin()));
            }

            _acessToken = MakeStringToken(_accessKey, expiresAfter ?? ExpiresDefault, claims.ToArray());

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
            _refreshToken = MakeStringToken(_refreshKey, expiresAfter ?? ExpiresDefault, claim);

            return this;
        }

        public JWTGenerator CreateTokenPair(User user)
        {
            CreateAcessToken(user);
            CreateRefreshToken(user.Email, RefreshExpiresDefault);
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
