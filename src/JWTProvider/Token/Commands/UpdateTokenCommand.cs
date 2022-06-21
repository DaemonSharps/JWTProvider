using JWTProvider.Models;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.Token.Commands
{
    public class UpdateTokenCommand : IRequest<TokenModel>
    {
        [Required]
        public Guid RefreshToken { get; set; }
    }
}
