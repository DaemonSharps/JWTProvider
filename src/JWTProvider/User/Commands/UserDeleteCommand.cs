using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace JWTProvider.User.Commands;

public class UserDeleteCommand : IRequest<Infrastructure.DataBase.Entities.User>
{
    [EmailAddress]
    public string Email { get; set; }
}

