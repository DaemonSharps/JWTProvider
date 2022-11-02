using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase.Context;
using Infrastructure.DataBase.Entities;
using Infrastructure.DataBase.Interfaces;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCSharp.HttpUserAgentParser.AspNetCore;

namespace JWTProvider.Session.Commands;

public class UploadUserAgentInfoHandler : IRequestHandler<UploadUserAgentInfoCommand, UserAgentDBEntries>
{
    private readonly IHttpUserAgentParserAccessor _parserAccessor;
    private readonly UsersDBContext _context;
    private readonly ILogger<UploadUserAgentInfoHandler> _logger;

    public UploadUserAgentInfoHandler(IHttpUserAgentParserAccessor parserAccessor, UsersDBContext context, ILogger<UploadUserAgentInfoHandler> logger)
    {
        _parserAccessor = parserAccessor;
        _context = context;
        _logger = logger;
    }

    public async Task<UserAgentDBEntries> Handle(UploadUserAgentInfoCommand request, CancellationToken cancellationToken)
    {
        var userAgentInfo = _parserAccessor.Get(request.HttpContext);

        var agentOSType = userAgentInfo?.Platform?.PlatformType.ToString();
        var osType = await CreateOrGet(_context.OperatingSystemTypes, agentOSType, cancellationToken);

        var agentAppType = userAgentInfo?.Type.ToString();
        var appType = await CreateOrGet(_context.AppTypes, agentAppType, cancellationToken);

        var agentAppName = userAgentInfo?.Name;
        var app = await CreateOrGet(_context.Apps, agentAppName, cancellationToken);

        if (appType != null)
        {
            app.AppType = appType;
        }

        var iPAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();

        var agentDBInfo = new UserAgentDBEntries
        {
            AppId = app?.Id,
            AppTypeId = appType?.Id,
            OperatingSystemTypeId = osType?.Id,
            IpAddress = iPAddress
        };
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(agentDBInfo);
            _logger.LogError(ex, "Upload agent info failed. Handler request: {JsonRequest}", jsonRequest);
            throw new UpdateSessionException("DB error", ex);
        }

        return agentDBInfo;
    }

    private static async Task<T> CreateOrGet<T>(DbSet<T> dBSet, string codeValue, CancellationToken cancellationToken)
        where T : class, ITypeEntity, new()
    {
        T entity = null;
        if (!string.IsNullOrEmpty(codeValue))
        {
            entity = await dBSet.FirstOrDefaultAsync(e => e.Code == codeValue, cancellationToken);
            if (entity == null)
            {
                entity = new T
                {
                    Code = codeValue
                };
                dBSet.Add(entity);
            }
        }

        return entity;
    }
}

