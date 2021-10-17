using System.ComponentModel.DataAnnotations;
using MediatR;
using Infrastructure.Entities;

namespace JWTProvider.Token.Commands
{
    public class GetTokenCommand : IRequest<(TokenModel model, RestApiError error)>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
