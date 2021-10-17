using Infrastructure.Entities;
using MediatR;

namespace JWTProvider.Token.Commands
{
    public class GetTokenCommand : IRequest<(TokenModel model, RestApiError error)>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
