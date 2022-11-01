using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace JWTProvider.User.Queries;

public class LoginUserCommand : IRequest<Infrastructure.DataBase.Entities.User>
{
    [EmailAddress, Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

