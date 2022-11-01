using System;
using MediatR;

namespace JWTProvider.User.Commands;

public class UserDeleteCommand : IRequest<Infrastructure.DataBase.Entities.User>
{
    public string Email { get; set; }
}

