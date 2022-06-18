using Infrastructure.CustomAttributes.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace Infrastructure.Middleware
{
    public static partial class ServiceCollectionExtentions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var apiInfo = new OpenApiInfo
            {
                Title = "JWTProvider",
                Version = "v1",
                Description = "Authorization provider for [DaemonSharps](https://github.com/DaemonSharps) apps"
            };

            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "bearer",
                Description = "Please insert JWT token into field"
            };

            var securityRequirment = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };

            return services.AddSwaggerGen(c =>
            {
                c.OperationFilter<CommandAttributeFilter>();
                c.OperationFilter<QuerryAttributeFilter>();
                c.EnableAnnotations();
                c.SwaggerDoc("v1", apiInfo);
                c.DescribeAllParametersInCamelCase();
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(securityRequirment);
            });
        }
    }
}
