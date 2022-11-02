using System;
using MediatR;

namespace JWTProvider.Session.Commands;

public class CreateSessionCommand : IRequest<Infrastructure.DataBase.Entities.Session>
{
    public Guid UserId { get; set; }

    public UserAgentDBEntries UserAgentInfo { get; set; }
}

