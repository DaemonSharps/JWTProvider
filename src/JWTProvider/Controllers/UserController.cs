using Infrastructure.Common;
using Infrastructure.Common.JWT;
using Infrastructure.Constants;
using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.Entities;
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
        [SwaggerResponse(400, "An error was occured", typeof(RestApiError))]
        public async Task<IActionResult> Registration([FromQuery] UserRegistrationCommand command, [FromServices] IOptions<TokenOptions> options)
        {
            var (user, userError) = await Mediator.Send(command);
            if (userError != null) return BadRequest(userError);

            var generator = JWTGenerator
                .GetGenerator(options.Value)
                .CreateTokenPair(user);

            Cache.Set(user.Email, generator.RefteshToken, JWTGenerator.RefreshExpiresDefault);

            return Ok(new TokenModel
            {
                Token = generator.AcessToken,
                RefreshToken = generator.RefteshToken
            });
        }

        [HttpPut, Command, Authorize]
        [SwaggerOperation("Update user public parameters")]
        [SwaggerResponse(200, "Update successfull, access token returned", typeof(string))]
        [SwaggerResponse(204, "No params to update")]
        [SwaggerResponse(400, "An error was occured", typeof(RestApiError))]
        public async Task<IActionResult> UpdateUser(string firstName, string middleName, string lastName, string login, [FromServices] IOptions<TokenOptions> options)
        {
            var cmd = new UserUpdateCommand
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Login = login,
                Email = User.GetEmail()
            };
            var (user, error) = await Mediator.Send(cmd);
            if (user is null) return error.Code switch
            {
                RestErrorCodes.NoContent => NoContent(),
                RestErrorCodes.UserNF => NotFound(error)
            };

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
