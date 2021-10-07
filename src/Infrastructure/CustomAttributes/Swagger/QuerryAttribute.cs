using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomAttributes.Swagger
{
    /// <summary>
    /// Атрибут для методов "Запросов"
    /// </summary>
    public class QuerryAttribute : CQRSAttribute
    {
        public QuerryAttribute()
        {
            Name = Constants.SwaggerTagNames.Queries;
        }
    }

    public class QuerryAttributeFilter : CQRSAttributeFilter<QuerryAttribute> { }
}
