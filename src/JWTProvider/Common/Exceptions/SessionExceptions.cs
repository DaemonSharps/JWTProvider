using System;
using System.Net;
using Infrastructure.Common.Exceptions;

namespace JWTProvider.Common.Exceptions;

public class SessionException : HttpResponseException
{
    public SessionException(string errorCode, string errorMessage = null, Exception innerException = null)
        : base(System.Net.HttpStatusCode.BadRequest, errorCode, errorMessage, innerException)
    { }
}

public class CreateSessionException : SessionException
{
    public CreateSessionException(string errorMessage = null, Exception innerException = null)
        : base("CREATE_SESSION_FAILED", errorMessage, innerException)
    { }
}

public class UpdateSessionException : SessionException
{
    public UpdateSessionException(string errorMessage = null, Exception innerException = null)
        : base("UPDATE_SESSION_FAILED", errorMessage, innerException)
    { }
}

