using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.Entities;
using JWTProvider.Models;
using JWTProvider.Token.Commands;
using JWTProvider.User.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Registration([FromQuery] UserRegistrationCommand command)
        {
            var (_, userError) = await Mediator.Send(command);
            if (userError != null) return BadRequest(userError);

            var cmd = new GetTokenCommand
            {
                Password = command.Password,
                Email = command.Email
            };
            var (token, tokenError) = await Mediator.Send(cmd);

            return token switch
            {
                null => BadRequest(tokenError),
                _ => Ok(token)
            };
        }
    }
}
