﻿using System;
using System.Threading.Tasks;
using Infrastructure.Common;
using Infrastructure.Common.Exceptions;
using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.Extentions;
using Infrastructure.Middleware;
using Infrastructure.Middleware.Options;
using JWTProvider.Models;
using JWTProvider.Session.Commands;
using JWTProvider.User.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace JWTProvider.Controllers;

public class UsersController : BaseController
{
    [HttpPost, Command, AllowAnonymous]
    [SwaggerOperation("User registration")]
    [SwaggerResponse(200, "Registration completed successfully", typeof(TokenModel))]
    [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
    public async Task<IActionResult> Registration(
        UserRegistrationCommand command,
        [FromServices] IOptions<TokenOptions> options,
        [FromServices] IMemoryCache cache)
    {
        var user = await Mediator.Send(command);

        var getAgentInfoCommand = new UploadUserAgentInfoCommand
        {
            HttpContext = Request.HttpContext
        };

        var userAgentInfoDB = await Mediator.Send(getAgentInfoCommand);
        var createSessionCommand = new CreateSessionCommand
        {
            UserId = user.Id,
            UserAgentInfo = userAgentInfoDB
        };

        var session = await Mediator.Send(createSessionCommand);

        var accessToken = JWTGenerator
            .GetGenerator(options.Value)
            .CreateAcessToken(user)
            .AcessToken;

        return Ok(new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = session.RefreshToken
        });
    }

    [HttpPut, Command, Authorize]
    [SwaggerOperation("Update user public parameters")]
    [SwaggerResponse(200, "Update successfull")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
    public async Task<IActionResult> UpdateUser(UserUpdateCommand command)
    {
        command.Email = User.GetEmail();
        await Mediator.Send(command);

        return Ok();
    }

    [HttpDelete, Command, Authorize]
    [SwaggerOperation("Close user account")]
    [SwaggerResponse(200, "Account close successfull")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(400, "An error was occured", typeof(ApiError))]
    public async Task<IActionResult> DeleteUser()
    {
        var command = new UserDeleteCommand
        {
            Email = User.GetEmail()
        };

        var result = await Mediator.Send(command);

        return Ok();
    }

    [HttpGet("pwd"), Querry, Authorize]
    public IActionResult GetUpdatePasswordUrl()
    {
        return new StatusCodeResult(501);
    }

    [HttpPut("pwd"), Command, Authorize]
    public IActionResult UpdatePassword()
    {
        return new StatusCodeResult(501);
    }
}
