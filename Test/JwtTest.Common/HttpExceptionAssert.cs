using Infrastructure.Common.Exceptions;
using Xunit;

namespace JwtTest.Common;

public static class HttpExceptionAssert
{
    public static void IsValidHttpException(HttpResponseException exception, System.Net.HttpStatusCode expectedStatusCode, string expectedErrorCode, string expectedErrorMessage)
    {
        Assert.Equal(expectedStatusCode, exception.StatusCode);
        var apiError = Assert.IsType<ApiError>(exception.Error);
        Assert.Equal(expectedErrorCode, apiError.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiError.ErrorMessage);
    }
}
