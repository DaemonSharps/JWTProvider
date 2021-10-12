using System.ComponentModel.DataAnnotations;
using MediatR;
using Infrastructure.Entities;

namespace JWTProvider.Token.Commands
{
    public class GetTokenCommand : IRequest<(TokenModel model, RestApiError error)>
    {
        [Required, EmailAddress]
        public string EMail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
