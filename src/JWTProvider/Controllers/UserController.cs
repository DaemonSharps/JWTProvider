using Infrastructure.Common.Exceptions;
using Infrastructure.Common;
using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.Extentions;
using Infrastructure.Middleware;
using JWTProvider.Models;
using JWTProvider.User.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using RT = Infrastructure.Constants.RefreshToken;

namespace JWTProvider.Controllers
{
    public class UserController : BaseController
    {
        [HttpPost, Command, AllowAnonymous]
        [SwaggerOperation("User registration")]
        [SwaggerResponse(200, "Registration completed successfully", typeof(TokenModel))]
        [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
        public async Task<IActionResult> Registration(UserRegistrationCommand command, [FromServices] IOptions<TokenOptions> options)
        {
            var user = await Mediator.Send(command);

            var accessToken = JWTGenerator
                .GetGenerator(options.Value)
                .CreateAcessToken(user)
                .AcessToken;
            var refreshToken = Guid.NewGuid();

            Cache.Set(refreshToken, user.Email, RT.ExpiresDefault);

            return Ok(new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPut, Command, Authorize]
        [SwaggerOperation("Update user public parameters")]
        [SwaggerResponse(200, "Update successfull")]
        [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
        public async Task<IActionResult> UpdateUser(UserUpdateCommand command, [FromServices] IOptions<TokenOptions> options)
        {
            command.Email = User.GetEmail();
            await Mediator.Send(command);

            return Ok();
        }

        [HttpGet("pwd"), Querry, Authorize]
        public async Task<IActionResult> GetUpdatePasswordUrl()
        {
            return new StatusCodeResult(501);
        }

        [HttpPut("pwd"), Command, Authorize]
        public async Task<IActionResult> UpdatePassword()
        {
            return new StatusCodeResult(501);
        }
    }
}
