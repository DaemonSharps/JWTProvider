using Infrastructure.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    public class LayerExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public LayerExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (LayerException ex)
            {

                context.Response.Clear();
                context.Response.StatusCode = (int)ex.StatusCode;
                context.Response.ContentType = ex.ContentType;
                await context.Response.WriteAsJsonAsync(ex.Error);
                return;
            }
        }
    }

    public static class LayerExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseLayerExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LayerExceptionMiddleware>();
        }
    }
}
