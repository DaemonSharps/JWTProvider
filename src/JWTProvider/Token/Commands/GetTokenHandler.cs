﻿using Infrastructure.Common;
using Infrastructure.Common.JWT;
using Infrastructure.Constants;
using Infrastructure.DataBase;
using Infrastructure.Entities;
using Infrastructure.Extentions;
using JWTProvider.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace JWTProvider.Token.Commands
{
    public class GetTokenHandler : IRequestHandler<GetTokenCommand, (TokenModel model, RestApiError error)>
    {
        private readonly IConfiguration _config;
        private readonly UsersDBContext _context;
        private readonly IMemoryCache _cache;

        private readonly TimeSpan _defaultRTLifetime = TimeSpan.FromDays(7);

        public GetTokenHandler(IConfiguration configuration, UsersDBContext dBContext, IMemoryCache memoryCache)
        {
            _config = configuration;
            _context = dBContext;
            _cache = memoryCache;
        }

        public async System.Threading.Tasks.Task<(TokenModel model, RestApiError error)> Handle(GetTokenCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Login)
                .Include(u => u.Password)
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
            if (user is null) return (null, new() { Code = RestErrorCodes.LoginFalied, Message = "User not found" });

            var hashedPassword = user?.HashPassword(command.Password);
            if (!hashedPassword.Equals(user.Password.Hash)) return (null, new() { Code = RestErrorCodes.LoginFalied, Message = "Invalid email or password" });

            var generator = JWTGenerator
                .GetGenerator(_config[ConfigurationKeys.AccessKey], _config[ConfigurationKeys.RefreshKey], _config[ConfigurationKeys.TokenIssuer])
                .CreateTokenPair(user);

            _cache.Set(user.Email, generator.RefteshToken, _defaultRTLifetime);

            return (new TokenModel
            {
                Token = generator.AcessToken,
                RefreshToken = generator.RefteshToken
            }, null);
        }
    }
}
