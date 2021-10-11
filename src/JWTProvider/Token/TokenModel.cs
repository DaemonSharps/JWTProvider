using System;
using Infrastructure.Entities;

namespace JWTProvider.Token
{
    public class TokenModel
    {
        public User User { get; set; }

        public string Token { get; set; }

        public string DisplayLogin { get; set; }
    }
}
