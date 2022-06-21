﻿using Infrastructure.Common.Exceptions;
using System;

namespace JWTProvider.Common.Exceptions
{
    public class UserNotFoundException : HttpResponseException
    {
        public UserNotFoundException(string errorMesage = "User not found", Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "USER_NOT_FOUND", errorMesage, innerException)
        { }
    }
    public class UserExistsException : HttpResponseException
    {
        public UserExistsException(string errorMesage = "User is already exist", Exception innerException = null)
            : base(System.Net.HttpStatusCode.BadRequest, "USER_EXISTS", errorMesage, innerException)
        { }
    }
}