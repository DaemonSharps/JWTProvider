using System;
using System.Threading.Tasks;
using Infrastructure.Common;
using Infrastructure.Common.Exceptions;
using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.DataBase.Entities;
using Infrastructure.Middleware.Options;
using JWTProvider.Models;
using JWTProvider.Session.Commands;
using JWTProvider.Token.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace JWTProvider.Controllers
{
    [AllowAnonymous]
    public class TokenController : BaseController
    {
        [HttpPost, Command]
        [SwaggerOperation("Get JsonWebToken")]
        [SwaggerResponse(200, "Authorization successsful", typeof(TokenModel))]
        [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
        public async Task<IActionResult> GetToken(GetTokenCommand request)
        {
            var model = await Mediator.Send(request);
            return Ok(model);
        }

        [HttpPut, Command]
        [SwaggerOperation("Check the token and get a new pair of JWT and RT")]
        [SwaggerResponse(200, "Token verified", typeof(TokenModel))]
        [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
        public async Task<IActionResult> CheckRefreshToken(UpdateSessionCommand command, [FromServices] IOptions<TokenOptions> options)
        {
            string accessToken = null;
            Guid? refreshToken = null;
            try
            {
                var session = await Mediator.Send(command);

                refreshToken = session.RefreshToken;
                accessToken = JWTGenerator
                    .GetGenerator(options.Value)
                    .CreateAcessToken(session.User)
                    .AcessToken;
            }
            catch (System.Exception ex)
            {

            }

            //TODO: избавиться от использования UpdateTokenHandler
            var tokenModel = await Mediator.Send(new UpdateTokenCommand { RefreshToken = command.RefreshToken });
            var model = new TokenModel
            {
                RefreshToken = tokenModel.RefreshToken,
                AccessToken = accessToken ?? tokenModel.AccessToken
            };
            return Ok(model);
        }
    }
}
