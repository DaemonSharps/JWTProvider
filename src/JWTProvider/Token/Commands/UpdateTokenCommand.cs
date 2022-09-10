using System;
using System.ComponentModel.DataAnnotations;
using JWTProvider.Models;
using MediatR;

namespace JWTProvider.Token.Commands;

public class UpdateTokenCommand : IRequest<TokenModel>
{
    [Required]
    public Guid RefreshToken { get; set; }
}
