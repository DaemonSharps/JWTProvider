using System;
namespace Infrastructure.Middleware.Options;

public class SessionOptions
{
    /// <summary>
    /// Секция в appsettings.json
    /// </summary>
    public const string Section = "Session";

    /// <summary>
    /// Время жизни рефреш сессии
    /// </summary>
    public TimeSpan Lifetime { get; set; }

    /// <summary>
    /// Максимальное количество сессий для одного приложения
    /// </summary>
    public int MaxSessionsCount { get; set; }
}

