using Infrastructure.Common;
using Infrastructure.DataBase;
using Infrastructure.Middleware;
using JWTProvider.Common.Exceptions;
using JWTProvider.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RT = Infrastructure.Constants.RefreshToken;

namespace JWTProvider.Token.Commands
{
    public class UpdateTokenHandler : IRequestHandler<UpdateTokenCommand, TokenModel>
    {
        private readonly UsersDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly IOptions<TokenOptions> _options;

        public UpdateTokenHandler(UsersDBContext dBContext, IMemoryCache memoryCache, IOptions<TokenOptions> options)
        {
            _context = dBContext;
            _cache = memoryCache;
            _options = options;
        }

        public async Task<TokenModel> Handle(UpdateTokenCommand request, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(request.RefreshToken, out var cachedEmail))
            {
                var user = await _context.Users
                        .Include(u => u.Password)
                        .SingleOrDefaultAsync(u => u.Email.Equals(cachedEmail), cancellationToken);

                var accessToken = JWTGenerator
                    .GetGenerator(_options.Value)
                    .CreateAcessToken(user)
                    .AcessToken;
                var refreshToken = Guid.NewGuid();

                _cache.Set(refreshToken, cachedEmail, RT.ExpiresDefault);

                return new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }

            throw new InvalidRefreshTokenException();
        }
    }
}
