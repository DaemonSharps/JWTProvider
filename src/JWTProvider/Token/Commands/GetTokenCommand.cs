using System.ComponentModel.DataAnnotations;
using MediatR;

namespace JWTProvider.Token.Commands
{
    public class GetTokenCommand : IRequest<(TokenModel model, string error)>
    {
        [Required, EmailAddress]
        public string EMail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
