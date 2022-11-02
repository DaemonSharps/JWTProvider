using System;
using System.Threading.Tasks;
using Infrastructure.Common;
using Infrastructure.Common.Exceptions;
using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.DataBase.Entities;
using Infrastructure.Middleware.Options;
using JWTProvider.Models;
using JWTProvider.Session.Commands;
using JWTProvider.User.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace JWTProvider.Controllers;

[AllowAnonymous]
public class SessionsController : BaseController
{
    [HttpPost, Command]
    [SwaggerOperation("Get JsonWebToken")]
    [SwaggerResponse(200, "Authorization successsful", typeof(TokenModel))]
    [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
    public async Task<IActionResult> GetToken(LoginUserCommand command, [FromServices] IOptions<TokenOptions> options)
    {
        var loggedUser = await Mediator.Send(command);

        var getAgentInfoCommand = new UploadUserAgentInfoCommand
        {
            HttpContext = Request.HttpContext
        };

        var userAgentInfoDB = await Mediator.Send(getAgentInfoCommand);
        var createSessionCommand = new CreateSessionCommand
        {
            UserId = loggedUser.Id,
            UserAgentInfo = userAgentInfoDB
        };

        var session = await Mediator.Send(createSessionCommand);

        var accessToken = JWTGenerator
                .GetGenerator(options.Value)
                .CreateAcessToken(session.User)
                .AcessToken;

        var model = new TokenModel
        {
            RefreshToken = session.RefreshToken,
            AccessToken = accessToken
        };

        return Ok(model);
    }

    [HttpPut, Command]
    [SwaggerOperation("Check the token and get a new pair of JWT and RT")]
    [SwaggerResponse(200, "Token verified", typeof(TokenModel))]
    [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
    public async Task<IActionResult> CheckRefreshToken(UpdateSessionCommand command, [FromServices] IOptions<TokenOptions> options)
    {
        var session = await Mediator.Send(command);

        var refreshToken = session.RefreshToken;
        var accessToken = JWTGenerator
            .GetGenerator(options.Value)
            .CreateAcessToken(session.User)
            .AcessToken;

        var model = new TokenModel
        {
            RefreshToken = session.RefreshToken,
            AccessToken = accessToken
        };
        return Ok(model);
    }
}
