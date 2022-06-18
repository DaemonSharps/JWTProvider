using Infrastructure.Common;
using Infrastructure.Common.JWT;
using Infrastructure.Constants;
using Infrastructure.DataBase;
using Infrastructure.Entities;
using Infrastructure.Middleware;
using JWTProvider.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JWTProvider.Token.Commands
{
    public class UpdateTokenHandler : IRequestHandler<UpdateTokenCommand, (TokenModel model, RestApiError error)>
    {
        private readonly UsersDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly IOptions<TokenOptions> _options;

        private readonly TimeSpan _defaultRTLifetime = TimeSpan.FromDays(7);

        public UpdateTokenHandler(UsersDBContext dBContext, IMemoryCache memoryCache, IOptions<TokenOptions> options)
        {
            _context = dBContext;
            _cache = memoryCache;
            _options = options;
        }

        public async Task<(TokenModel model, RestApiError error)> Handle(UpdateTokenCommand request, CancellationToken cancellationToken)
        {
            var accessKey = _options.Value?.AccessKey;
            var refreshKey = _options.Value?.RefreshKey;
            var tokenIssuer = _options.Value?.Issuer;

            JwtPayload payload = null;
            try
            {
                payload = JWTValidator.GetValidator(refreshKey, tokenIssuer)
                    .ValidateRefreshToken(request.Token)
                    .TokenPayload;
            }
            catch (SecurityTokenValidationException ex)
            {
                return (null, new() { Code = RestErrorCodes.InvalidRT, Message = ex.Message });
            }

            var email = payload.Claims?.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            if (_cache.TryGetValue(email, out var cachedToken))
            {
                if (request.Token.Equals(cachedToken))
                {
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .Include(u => u.Password)
                        .Include(u => u.Login)
                        .SingleOrDefaultAsync(u => u.Email.Equals(email), cancellationToken);


                    var generator = JWTGenerator
                        .GetGenerator(accessKey, refreshKey, tokenIssuer)
                        .CreateTokenPair(user);
                    _cache.Set(email, generator.RefteshToken, _defaultRTLifetime);

                    return (new()
                    {
                        Token = generator.AcessToken,
                        RefreshToken = generator.RefteshToken
                    }, null);
                }
            }

            return (null, new() { Code = RestErrorCodes.InvalidRT, Message = "Invalid token" });
        }
    }
}
