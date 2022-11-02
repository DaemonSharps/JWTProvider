using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase.Context;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DB = Infrastructure.DataBase.Entities;

namespace JWTProvider.Session.Commands;

public class UpdateSessionHandler : IRequestHandler<UpdateSessionCommand, DB.Session>
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UpdateSessionHandler> _logger;

    public UpdateSessionHandler(UsersDBContext context, ILogger<UpdateSessionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DB.Session> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        var currentSession = await _context
            .Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken, cancellationToken);
        if (currentSession == null || currentSession.FinishDate < DateTimeOffset.UtcNow)
        {
            throw new UpdateSessionException("Session not found");
        }

        currentSession.RefreshToken = Guid.NewGuid();
        currentSession.FinishDate = DateTimeOffset.UtcNow.AddDays(10);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _logger.LogError(ex, "Update session failed. Handler request: {JsonRequest}", jsonRequest);
            throw new UpdateSessionException("DB error", ex);
        }
        return currentSession;
    }
}

