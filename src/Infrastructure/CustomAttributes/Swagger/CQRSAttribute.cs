﻿using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.CustomAttributes.Swagger
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public abstract class CQRSAttribute : Attribute
    {
        public string Name { get; set; }

        public string ControllerName { get; set; }
    }

    public abstract class CQRSAttributeFilter<T> : IOperationFilter where T : CQRSAttribute
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<T>();
            var controllerName = context.MethodInfo.DeclaringType.Name;

            foreach (var attribute in attributes)
            {
                attribute.ControllerName = controllerName[0..^10];
                EnrichOperationWithAttribute(operation, attribute);
            }
        }

        private static void EnrichOperationWithAttribute(OpenApiOperation operation, T attribute)
        {
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                operation.Tags = new[] {
                    new OpenApiTag
                    {
                        Name = $"{attribute.Name} operations for {attribute.ControllerName}"
                    }
                };
            }
        }
    }
}
