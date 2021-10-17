using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.Entities;
using JWTProvider.Token;
using JWTProvider.Token.Commands;
using JWTProvider.User;
using JWTProvider.User.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace JWTProvider.Controllers
{
    public class UserController : BaseController
    {
        internal class RegistrationModel
        {
            public UserModel User { get; set; }

            public TokenModel Token { get; set; }
        }

        [HttpPost, Command, AllowAnonymous]
        [SwaggerOperation("Регистрация пользователя")]
        [SwaggerResponse(200, "Регистрация прошла успешно", typeof(RegistrationModel))]
        [SwaggerResponse(400, "Произошла ошибка", typeof(RestApiError))]
        public async Task<IActionResult> Registration([FromQuery] UserRegistrationCommand command)
        {
            var (user, userError) = await Mediator.Send(command);
            if (userError != null) return BadRequest(userError);
            var cmd = new GetTokenCommand 
            {
                Password = command.Password,
                Email = command.Email
            };
            var (token, tokenError) = await Mediator.Send(cmd);
            if (tokenError != null) return BadRequest(tokenError);

            return Ok(new RegistrationModel
            {
                User = user,
                Token = token
            });
        }
    }
}
