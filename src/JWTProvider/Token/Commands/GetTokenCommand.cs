using JWTProvider.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.Token.Commands
{
    public class GetTokenCommand : IRequest<TokenModel>
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
