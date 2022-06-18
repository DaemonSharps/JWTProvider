using Microsoft.Extensions.Configuration;

namespace Infrastructure.Middleware
{
    public static partial class ConfigurationExtentions
    {
        public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string section)
            => configuration
            .GetSection(section)
            .Get<TOptions>();
    }
}
