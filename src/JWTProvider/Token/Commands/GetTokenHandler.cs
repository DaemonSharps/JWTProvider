using System;
using System.Threading;
using Infrastructure.Common;
using Infrastructure.DataBase;
using Infrastructure.Extentions;
using Infrastructure.Middleware;
using JWTProvider.Common.Exceptions;
using JWTProvider.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace JWTProvider.Token.Commands
{
    public class GetTokenHandler : IRequestHandler<GetTokenCommand, TokenModel>
    {
        private readonly UsersDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly IOptions<TokenOptions> _options;

        private readonly TimeSpan _defaultRTLifetime = TimeSpan.FromDays(7);

        public GetTokenHandler(UsersDBContext dBContext, IMemoryCache memoryCache, IOptions<TokenOptions> options)
        {
            _context = dBContext;
            _cache = memoryCache;
            _options = options;
        }

        public async System.Threading.Tasks.Task<TokenModel> Handle(GetTokenCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Password)
                .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
            if (user is null) throw new LoginFailedException("User not found");

            var hashedPassword = user?.HashPassword(command.Password);
            if (!hashedPassword.Equals(user.Password.Hash)) throw new LoginFailedException("Invalid email or password");

            var accessToken = JWTGenerator
                .GetGenerator(_options.Value)
                .CreateAcessToken(user)
                .AcessToken;
            var refreshToken = Guid.NewGuid();
            _cache.Set(refreshToken, user.Email, _defaultRTLifetime);

            return new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
