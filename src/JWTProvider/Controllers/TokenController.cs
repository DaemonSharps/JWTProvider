using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.Entities;
using JWTProvider.Token;
using JWTProvider.Token.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace JWTProvider.Controllers
{
    public class TokenController : BaseController
    {
        #region Commands

        [HttpPost, Command, AllowAnonymous]
        [SwaggerOperation("Получить токен JWT")]
        [SwaggerResponse(200, "Авторизация прошла успешно", typeof(TokenModel))]
        [SwaggerResponse(400, "Произошла ошибка", typeof(RestApiError))]
        public async Task<IActionResult> GetToken([FromQuery] GetTokenCommand request)
        {
            var (model, error) = await Mediator.Send(request);

            return model switch
            {
                null => NotFound(error),
                _ => Ok(model)
            };
        }

        [HttpPut, Command, AllowAnonymous]
        [SwaggerOperation("Проверить Refresh Token и получить новую пару значений JWT RT")]
        [SwaggerResponse(200, "Токен проверен успешно", typeof(TokenModel))]
        [SwaggerResponse(400, "Произошла ошибка", typeof(RestApiError))]
        public async Task<IActionResult> CheckRefreshToken([FromQuery] UpdateTokenCommand command)
        {
            var (model, error) = await Mediator.Send(command);

            return model switch
            {
                null => BadRequest(error),
                _ => Ok(model)
            };
        }

        #endregion
    }
}
