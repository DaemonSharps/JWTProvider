using Infrastructure.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTProvider.Common.Exceptions
{
    public abstract class TokenException : LayerException
    {
        public TokenException(string errorCode, string errorMesage)
            : base(System.Net.HttpStatusCode.BadRequest, errorCode, errorMesage)
        {

        }

        public override string Layer => 
            base.Layer + " -> Token";
    }

    public class LoginFailedException : TokenException
    {
        public LoginFailedException(string errorMessage)
            : base("LOGIN_FAILED", errorMessage)
        { }
    }

    public class InvalidRefreshTokenException : TokenException
    {
        public InvalidRefreshTokenException(string errorMessage)
            : base("INVALID_REFRESH_TOKEN", errorMessage)
        { }
    }
}
