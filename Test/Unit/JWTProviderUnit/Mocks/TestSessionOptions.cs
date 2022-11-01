
using System;
using Infrastructure.Middleware.Options;
using Microsoft.Extensions.Options;

namespace Mocks;

internal class TestSessionOptions : IOptions<SessionOptions>
{
    public static TimeSpan Lifetime => TimeSpan.Parse("10.00:00:00");
    public const int MaxSessionsCount = 5;

    public SessionOptions Value => new SessionOptions
    {
        Lifetime = Lifetime,
        MaxSessionsCount = MaxSessionsCount
    };
}

