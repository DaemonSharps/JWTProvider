using System.Threading.Tasks;
using Infrastructure.Common.Exceptions;
using Infrastructure.CustomAttributes.Swagger;
using JWTProvider.Models;
using JWTProvider.Token.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CheckRefreshToken(UpdateTokenCommand command)
        {
            var model = await Mediator.Send(command);
            return Ok(model);
        }
    }
}
