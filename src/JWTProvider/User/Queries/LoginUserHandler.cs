using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase.Context;
using Infrastructure.Extentions;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DB = Infrastructure.DataBase.Entities;

namespace JWTProvider.User.Queries;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, DB.User>
{
    private readonly UsersDBContext _context;

    public LoginUserHandler(UsersDBContext dBContext)
    {
        _context = dBContext;
    }

    public async Task<DB.User> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Password)
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
        if (user is null || user.FinishDate < DateTimeOffset.UtcNow) throw new LoginFailedException("User not found");

        string hashedPassword;
        try
        {
            hashedPassword = user.HashPassword(command.Password);
            if (!hashedPassword.Equals(user.Password.Hash)) throw new LoginFailedException("Invalid email or password");
        }
        catch (ArgumentException ex)
        {
            throw new LoginFailedException("Invalid email or password", ex);
        }

        return user;
    }
}

