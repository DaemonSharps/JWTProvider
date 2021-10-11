using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.CustomAttributes.Swagger;
using JWTProvider.Token.Commands;
using MediatR;
using Infrastructure.DataBase;
using Infrastructure.Extentions;

namespace JWTProvider.Controllers
{
    public class TokenController : BaseController
    {
        public TokenController(IMediator mediator) : base(mediator) { }

        #region Querries

        [HttpGet, Querry]
        [SwaggerOperation("Проверить актуальность токена")]
        public async Task<IActionResult> CheckToken()
        {
            return Ok();
        }

        #endregion

        #region Commands

        [HttpPost, Command]
        [SwaggerOperation("Получить токен JWT")]
        public async Task<IActionResult> GetToken(GetTokenCommand request)
        {
            var (model, error) = await _mediator.Send(request);
            IActionResult response = model switch
            {
                null => NotFound(error),
                _ => Ok(model)
            };

            return response;
        }

        [HttpPut, Command]
        [SwaggerOperation("Разлогиниться")]
        public async Task<IActionResult> LogOut()
        {
            return Ok();
        }

        #endregion
    }
}
