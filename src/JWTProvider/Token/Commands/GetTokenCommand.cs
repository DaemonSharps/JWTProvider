using System.ComponentModel.DataAnnotations;
using JWTProvider.Models;
using MediatR;

namespace JWTProvider.Token.Commands;

public class GetTokenCommand : IRequest<TokenModel>
{
    [EmailAddress, Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
