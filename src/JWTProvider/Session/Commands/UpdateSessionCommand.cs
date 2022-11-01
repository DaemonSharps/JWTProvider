using System;
using MediatR;

namespace JWTProvider.Session.Commands;

public class UpdateSessionCommand : IRequest<Infrastructure.DataBase.Entities.Session>
{
    public Guid RefreshToken { get; set; }
}

