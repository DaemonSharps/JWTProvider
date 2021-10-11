using System;
using Infrastructure.Entities;

namespace JWTProvider.Token
{
    public class TokenModel
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
