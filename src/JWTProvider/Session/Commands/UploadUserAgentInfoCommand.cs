using System;
using Microsoft.AspNetCore.Http;
using MediatR;

namespace JWTProvider.Session.Commands;

public class UploadUserAgentInfoCommand : IRequest<UserAgentDBEntries>
{
    public HttpContext HttpContext { get; set; }
}

public class UserAgentDBEntries
{
    public Guid? OperatingSystemTypeId { get; set; }

    public Guid? AppId { get; set; }

    public Guid? AppTypeId { get; set; }

    public string IpAddress { get; set; }
}

