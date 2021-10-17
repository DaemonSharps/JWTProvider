using Infrastructure.Common;
using Infrastructure.Common.JWT;
using Infrastructure.Constants;
using Infrastructure.DataBase;
using Infrastructure.Entities;
using JWTProvider.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        private readonly UsersDBContext _context;
        private readonly IMemoryCache _cache;

        private readonly TimeSpan _defaultRTLifetime = TimeSpan.FromDays(7);

        public UpdateTokenHandler(IConfiguration configuration, UsersDBContext dBContext, IMemoryCache memoryCache)
        {
            _config = configuration;
            _context = dBContext;
            _cache = memoryCache;
        }

        public async Task<(TokenModel model, RestApiError error)> Handle(UpdateTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenKey = _config[ConfigurationKeys.TokenKey];
            var tokenIssuer = _config[ConfigurationKeys.TokenIssuer];

            JwtPayload payload = null;
            try
            {
                payload = JWTValidator.GetValidator(tokenKey, tokenIssuer)
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
                        .GetGenerator(tokenKey, tokenIssuer)
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
