using System.Threading;
using MediatR;
using Infrastructure.DataBase;
using Infrastructure.Extentions;
using Infrastructure.Common;
using Infrastructure.Common.JWT;
using Infrastructure.Entities;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace JWTProvider.Token.Commands
{
    public class GetTokenHandler : IRequestHandler<GetTokenCommand, (TokenModel model, RestApiError error)>
    {
        private readonly IConfiguration _config;
        private readonly UsersDBContext _context;

        public GetTokenHandler(IConfiguration configuration, UsersDBContext dBContext)
        {
            _config = configuration;
            _context = dBContext;
        }

        public async System.Threading.Tasks.Task<(TokenModel model, RestApiError error)> Handle(GetTokenCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Login)
                .Include(u => u.Password)
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.EMail == command.EMail, cancellationToken);
            if (user is null) return (null, new() { Message = "User not found"});

            var hashedPassword = user?.HashPassword(command.Password);
            if (!hashedPassword.Equals(user.Password.Hash)) return (null, new() { Message = "Invalid email or password" });

            var tokenGenerator = new JWTGenerator(_config[ConfigurationKeys.TokenKey]);
            var token = tokenGenerator.CreateToken(user);

            return (new TokenModel
            {
                Token = token,
                RefreshToken = Guid.NewGuid().ToString() // todo: убрать временное решение
            }, null);
        }
    }
}
