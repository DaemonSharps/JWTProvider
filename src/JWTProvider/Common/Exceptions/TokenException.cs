using Infrastructure.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTProvider.Common.Exceptions
{
    public abstract class TokenException : HttpResponseException
    {
        public TokenException(string errorCode, string errorMesage = null, Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, errorCode, errorMesage, innerException)
        { }
    }

    public class LoginFailedException : TokenException
    {
        public LoginFailedException(string errorMessage = null, Exception innerException = null)
            : base("LOGIN_FAILED", errorMessage, innerException)
        { }
    }

    public class InvalidRefreshTokenException : TokenException
    {
        public InvalidRefreshTokenException(string errorMessage = null, Exception innerException = null)
            : base("INVALID_REFRESH_TOKEN", errorMessage, innerException)
        { }
    }
}
