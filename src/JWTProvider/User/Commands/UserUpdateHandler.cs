﻿using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase;
using Infrastructure.DataBase.Context;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DB = Infrastructure.DataBase.Entities;

namespace JWTProvider.User.Commands;

public class UserUpdateHandler : IRequestHandler<UserUpdateCommand, DB.User>
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UserUpdateHandler> _logger;

    public UserUpdateHandler(UsersDBContext context, ILogger<UserUpdateHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DB.User> Handle(UserUpdateCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null) throw new UserNotFoundException();

        user.FirstName = command.FirstName ?? user.FirstName;
        user.LastName = command.LastName ?? user.LastName;
        user.Patronymic = command.Patronymic ?? user.Patronymic;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(command);
            _logger.LogError(ex, "Update user failed. Handler request: {jsonRequest}", jsonRequest);
            throw new UserUpdateException("DB error", ex);
        }

        return user;
    }
}
