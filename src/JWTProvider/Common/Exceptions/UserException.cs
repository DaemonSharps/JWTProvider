using System;
using Infrastructure.Common.Exceptions;

namespace JWTProvider.Common.Exceptions
{
    public class UserNotFoundException : HttpResponseException
    {
        public UserNotFoundException(string errorMesage = "User not found", Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "USER_NOT_FOUND", errorMesage, innerException)
        { }
    }

    public class UserRegistrationException : HttpResponseException
    {
        public UserRegistrationException(string errorMesage, Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "USER_REGISTRATION_FAILED", errorMesage, innerException)
        { }
    }

    public class UserExistsException : HttpResponseException
    {
        public UserExistsException(string errorMesage = "User is already exist", Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "USER_EXISTS", errorMesage, innerException)
        { }
    }

    public class UserUpdateException : HttpResponseException
    {
        public UserUpdateException(string errorMesage, Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "USER_UPDATE_FAILED", errorMesage, innerException)
        { }
    }


    public class LoginFailedException : HttpResponseException
    {
        public LoginFailedException(string errorMessage = null, Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "LOGIN_FAILED", errorMessage, innerException)
        { }
    }
}
