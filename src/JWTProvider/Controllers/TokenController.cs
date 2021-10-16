using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using Infrastructure.CustomAttributes.Swagger;
using JWTProvider.Token.Commands;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace JWTProvider.Controllers
{
    public class TokenController : BaseController
    {
        #region Commands

        [HttpPost, Command, AllowAnonymous]
        [SwaggerOperation("Получить токен JWT")]
        public async Task<IActionResult> GetToken(GetTokenCommand request)
        {
            var (model, error) = await Mediator.Send(request);
            IActionResult response = model switch
            {
                null => NotFound(error),
                _ => Ok(model)
            };

            if (error == null) Cache.Set(request.Email, model.RefreshToken, TimeSpan.FromDays(7));

            return response;
        }

        [HttpPut, Command, Authorize]
        [SwaggerOperation("Проверить Refresh Token и получить новую пару значений JWT RT")]
        public async Task<IActionResult> CheckRefreshToken()
        {
            return Ok();
        }

        #endregion
    }
}
