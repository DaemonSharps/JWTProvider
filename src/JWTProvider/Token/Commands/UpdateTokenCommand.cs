using Infrastructure.Entities;
using JWTProvider.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.Token.Commands
{
    public class UpdateTokenCommand : IRequest<TokenModel>
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
