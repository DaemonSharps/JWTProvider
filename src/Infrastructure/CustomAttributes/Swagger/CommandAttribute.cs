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
    /// Атрибут для методов "Команд"
    /// </summary>
    public class CommandAttribute : CQRSAttribute
    {
        public CommandAttribute()
        {
            Name = Constants.SwaggerTagNames.Commands;
        }
    }

    public class CommandAttributeFilter : CQRSAttributeFilter<CommandAttribute> { }
}
