using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using Infrastructure.CustomAttributes.Swagger;
using JWTProvider.Token.Commands;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Common.JWT;
using Microsoft.Extensions.Configuration;
using Infrastructure.Common;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using System.Net;
using JWTProvider.Token;
using Infrastructure.Entities;

namespace JWTProvider.Controllers
{
    public class TokenController : BaseController
    {
        #region Commands

        [HttpPost, Command, AllowAnonymous]
        [SwaggerOperation("Получить токен JWT")]
        [SwaggerResponse(200, "Авторизация прошла успешно", typeof(TokenModel))]
        [SwaggerResponse(400, "Произошла ошибка", typeof(RestApiError))]
        public async Task<IActionResult> GetToken([EmailAddress, Required] string email, [Required]string password)
        {
            var request = new GetTokenCommand 
            {
                Email = email,
                Password = password
            };
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
        public async Task<IActionResult> CheckRefreshToken([Required]string refreshToken)
        {
            var cmd = new UpdateTokenCommand { Token = refreshToken };
            var (model, error) = await Mediator.Send(cmd);

            return model switch
            {
                null => BadRequest(error),
                _ => Ok(model)
            };
        }

        #endregion
    }
}
