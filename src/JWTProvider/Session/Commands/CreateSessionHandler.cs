using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.Exceptions;
using Infrastructure.DataBase.Context;
using JWTProvider.Common.Exceptions;
using JWTProvider.User.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DB = Infrastructure.DataBase.Entities;

namespace JWTProvider.Session.Commands;

public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, DB.Session>
{
    private readonly UsersDBContext _context;
    private readonly ILogger<CreateSessionHandler> _logger;

    public CreateSessionHandler(UsersDBContext context, ILogger<CreateSessionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DB.Session> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var app = await _context.Apps.FirstOrDefaultAsync(cancellationToken);
        var operatingSystemType = await _context.OperatingSystemTypes.FirstOrDefaultAsync(cancellationToken);
        var sessions = await _context
            .Sessions
            .Where(s => s.UserId == request.UserId && s.AppId == app.Id)
            .ToArrayAsync(cancellationToken);

        if (sessions.Length >= 5)
        {
            throw new CreateSessionException($"Max session count is limited: Max = 5, App = {app.Name}");
        }

        var session = new DB.Session
        {
            RefreshToken = Guid.NewGuid(),
            UserId = request.UserId,
            IP = "0.0.0.0",
            AppId = app.Id,
            OperatingSystemTypeId = operatingSystemType.Id
        };

        _context.Add(session);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _logger.LogError(ex, "Create session failed. Handler request: {JsonRequest}", request);
            throw new CreateSessionException("DB error", ex);
        }

        return session;
    }
}

