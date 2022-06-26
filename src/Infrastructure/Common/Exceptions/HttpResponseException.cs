using System;
using System.Net;

namespace Infrastructure.Common.Exceptions
{
    public abstract class HttpResponseException : Exception
    {
        private readonly string _errorCode;

        public HttpResponseException(HttpStatusCode statusCode, string errorCode, string errorMessage = null, Exception innerException = null)
           : base(errorMessage, innerException)
        {
            StatusCode = statusCode;
            _errorCode = errorCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        public virtual string ContentType => @"application/json; charset=utf-8";

        public virtual object Error => new ApiError
        {
            ErrorCode = _errorCode,
            ErrorMessage = Message
        };
    }

    public class ApiError
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
