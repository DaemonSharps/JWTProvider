using System;

namespace Infrastructure.Middleware.Options;

public class TokenOptions
{
    /// <summary>
    /// Секция в appsettings.json
    /// </summary>
    public const string Section = "Token";

    /// <summary>
    /// Ключ для генерации токена доступа
    /// </summary>
    public string AccessKey { get; set; }

    /// <summary>
    /// Униальный ключ издателя токена
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// время жизни токена
    /// </summary>
    public TimeSpan Lifetime { get; set; }
}
