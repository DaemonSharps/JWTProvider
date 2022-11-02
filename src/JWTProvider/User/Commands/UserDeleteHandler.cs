using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase.Context;
using JWTProvider.Session.Commands;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DB = Infrastructure.DataBase.Entities;
using System.Text.Json;
using System.Linq;

namespace JWTProvider.User.Commands;

public class UserDeleteHandler : IRequestHandler<UserDeleteCommand, DB.User>
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UserDeleteHandler> _logger;

    public UserDeleteHandler(UsersDBContext context, ILogger<UserDeleteHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DB.User> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
    {
        var user = await _context
            .Users
            .Include(u => u.Password)
            .Include(u => u.Sessions)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || user.FinishDate < DateTimeOffset.UtcNow)
        {
            throw new UserNotFoundException();
        }

        _context.Remove(user.Password);
        _context.Remove(user);
        _context.RemoveRange(user.Sessions);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _logger.LogError(ex, "Close user failed. Handler request: {JsonRequest}", jsonRequest);
            throw new UserUpdateException("DB error", ex);
        }

        return user;
    }
}

