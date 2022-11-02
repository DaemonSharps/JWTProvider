using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.Exceptions;
using Infrastructure.DataBase.Context;
using Infrastructure.Middleware.Options;
using JWTProvider.Common.Exceptions;
using JWTProvider.User.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DB = Infrastructure.DataBase.Entities;

namespace JWTProvider.Session.Commands;

public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, DB.Session>
{
    private readonly UsersDBContext _context;
    private readonly ILogger<CreateSessionHandler> _logger;
    private readonly IOptions<SessionOptions> _options;

    public CreateSessionHandler(
        UsersDBContext context,
        ILogger<CreateSessionHandler> logger,
        IOptions<SessionOptions> options)
    {
        _context = context;
        _logger = logger;
        _options = options;
    }

    public async Task<DB.Session> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var sessions = await _context
            .Sessions
            .Include(s => s.App)
            .Where(s => s.UserId == request.UserId && s.AppId == request.UserAgentInfo.AppId)
            .ToArrayAsync(cancellationToken);

        var maxSessionsCount = _options.Value.MaxSessionsCount;
        if (sessions.Length >= maxSessionsCount)
        {
            var appCode = sessions.First().App.Code;
            throw new CreateSessionException($"Max session count is limited: Max = {maxSessionsCount}, App = {appCode}");
        }

        var session = new DB.Session
        {
            RefreshToken = Guid.NewGuid(),
            UserId = request.UserId,
            IP = request.UserAgentInfo.IpAddress,
            AppId = request.UserAgentInfo.AppId,
            OperatingSystemTypeId = request.UserAgentInfo.OperatingSystemTypeId,
            FinishDate = DateTimeOffset.UtcNow.Add(_options.Value.Lifetime)
        };

        _context.Add(session);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _logger.LogError(ex, "Create session failed. Handler request: {JsonRequest}", jsonRequest);
            throw new CreateSessionException("DB error", ex);
        }

        return session;
    }
}

