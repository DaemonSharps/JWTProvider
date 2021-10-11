using System.Threading;
using MediatR;
using Infrastructure.DataBase;
using Infrastructure.Extentions;
using Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace JWTProvider.Token.Commands
{
    public class GetTokenHandler : IRequestHandler<GetTokenCommand, (TokenModel model, string error)>
    {
        private readonly IConfiguration _config;
        private readonly UsersDBContext _context;

        public GetTokenHandler(IConfiguration configuration, UsersDBContext dBContext)
        {
            _config = configuration;
            _context = dBContext;
        }

        public async System.Threading.Tasks.Task<(TokenModel model, string error)> Handle(GetTokenCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Login)
                .Include(u => u.Password)
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.EMail == command.EMail, cancellationToken);
            if (user is null) return (null, "User not found");

            var hashedPassword = user?.HashPassword(command.Password);
            if (!hashedPassword.Equals(user.Password.Hash)) return (null, "Invalid email or password");

            var tokenGenerator = new JWTGenerator(_config[ConfigurationKeys.TokenKey]);
            var token = tokenGenerator.CreateToken(user);

            return (new TokenModel
            {
                Token = token,
                User = user,
                DisplayLogin = user.Login?.ChangeableLogin
            }, null);
        }
    }
}
