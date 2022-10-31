using System;
using MediatR;

namespace JWTProvider.Session.Commands;

public class SessionCreateCommand : IRequest<Infrastructure.DataBase.Entities.Session>
{
    public Guid UserId { get; set; }
}

