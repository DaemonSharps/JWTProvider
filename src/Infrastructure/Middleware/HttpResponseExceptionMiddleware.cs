using Infrastructure.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    public class HttpResponseExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<HttpResponseExceptionMiddleware> _logger;

        public HttpResponseExceptionMiddleware(RequestDelegate next, ILogger<HttpResponseExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpResponseException ex)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)ex.StatusCode;
                context.Response.ContentType = ex.ContentType;
                await context.Response.WriteAsJsonAsync(ex.Error);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, $"{ex.Message} because of {ex.InnerException.Message}");
                }
                return;
            }
        }
    }

    public static class HttpResponseExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpResponseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpResponseExceptionMiddleware>();
        }
    }
}
