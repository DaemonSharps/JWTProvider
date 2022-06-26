using System.Net;
using System.Text.Json;
using DeepEqual.Syntax;
using Infrastructure.Common.Exceptions;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Middleware;

public class HttpResponseExceptionMiddlwareTests
{
    private class TestException : HttpResponseException
    {
        public TestException(HttpStatusCode statusCode, string errorCode, string errorMessage = null, Exception innerException = null)
            : base(statusCode, errorCode, errorMessage, innerException)
        {
        }
    }

    [Fact]
    public async Task DelegareThrowException()
    {
        //Arrange
        const string ErrorCode = "test_code";
        const string ErrorMessage = "test_message";
        var expectedApiError = new ApiError
        {
            ErrorCode = ErrorCode,
            ErrorMessage = ErrorMessage
        };
        var exception = new TestException(HttpStatusCode.BadRequest, ErrorCode, ErrorMessage);
        RequestDelegate requestDelegate = context
            => Task.FromException(exception);
        var middlware = new HttpResponseExceptionMiddleware(requestDelegate, new NullLogger<HttpResponseExceptionMiddleware>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        //Act
        await middlware.InvokeAsync(context);

        //Assert
        var response = context.Response;
        await AssertResponse(response, expectedApiError, exception.ContentType);
    }

    [Fact]
    public async Task DelegareThrowExceptionWithInner()
    {
        //Arrange
        const string ErrorCode = "test_code";
        const string ErrorMessage = "test_message";
        var expectedApiError = new ApiError
        {
            ErrorCode = ErrorCode,
            ErrorMessage = ErrorMessage
        };
        var innerException = new ArgumentException("innerException");
        var exception = new TestException(HttpStatusCode.BadRequest, ErrorCode, ErrorMessage, innerException);
        RequestDelegate requestDelegate = context
            => Task.FromException(exception);
        var loggerMock = new Mock<ILogger<HttpResponseExceptionMiddleware>>();
        var middlware = new HttpResponseExceptionMiddleware(requestDelegate, loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        //Act
        await middlware.InvokeAsync(context);

        //Assert
        var response = context.Response;
        await AssertResponse(response, expectedApiError, exception.ContentType);

        var invocation = loggerMock.Invocations.Single();
        Assert.Equal(LogLevel.Error, invocation.Arguments[0]);
        Assert.Contains(invocation.Arguments[2].ToString(), $"{exception.Message} because of {innerException.Message}");
        Assert.Equal(innerException, invocation.Arguments[3]);
    }

    private static async Task AssertResponse<TBody>(HttpResponse response, TBody expected, string contentType)
    {
        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(contentType, response.ContentType);
        response.Body.Position = 0;
        var apiErrorResponse = await JsonSerializer.DeserializeAsync<TBody>(response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        expected.IsDeepEqual(apiErrorResponse);
    }
}
