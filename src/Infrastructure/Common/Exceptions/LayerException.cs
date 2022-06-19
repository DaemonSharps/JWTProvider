using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Exceptions
{
    public abstract class LayerException : Exception
    {
        public LayerException(HttpStatusCode statusCode, string errorCode, string errorMessage)
            : base(errorMessage)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public virtual string ContentType => @"text/plain";

        public virtual string Layer { get; } = "JWTProvider";

        public HttpStatusCode StatusCode { get; set; }

        private string ErrorCode { get; set; }

        public ApiError Error 
        { 
            get 
            {
                return new()
                {
                    ErrorCode = ErrorCode,
                    ErrorMessage = Message
                };
            }
        }
    }

    public class ApiError
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
