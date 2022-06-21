using Infrastructure.Common.Exceptions;
using Infrastructure.Common.JWT;
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
using System.Threading.Tasks;

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

            var generator = JWTGenerator
                .GetGenerator(options.Value)
                .CreateTokenPair(user);

            Cache.Set(user.Email, generator.RefteshToken, JWTGenerator.RefreshExpiresDefault);

            return Ok(new TokenModel
            {
                AccessToken = generator.AcessToken,
                RefreshToken = generator.RefteshToken
            });
        }

        [HttpPut, Command, Authorize]
        [SwaggerOperation("Update user public parameters")]
        [SwaggerResponse(200, "Update successfull, access token returned", typeof(string))]
        [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
        public async Task<IActionResult> UpdateUser(UserUpdateCommand command, [FromServices] IOptions<TokenOptions> options)
        {
            command.Email = User.GetEmail();
            var user = await Mediator.Send(command);

            var generator = JWTGenerator
                .GetGenerator(options.Value)
                .CreateAcessToken(user);

            return Ok(generator.AcessToken);
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
