using Infrastructure.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTProvider.Token.Commands
{
    public class UpdateTokenCommand : IRequest<(TokenModel model, RestApiError error)>
    {
        [Required]
        public string Token { get; set; }
    }
}
