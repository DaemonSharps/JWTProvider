using Infrastructure.Middleware.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Middleware.ServiceCollectionExtentions
{
    public static partial class ServiceCollectionExtentions
    {
        public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfigurationOption<TokenOptions>(configuration, TokenOptions.Section);

            return services;
        }

        private static IServiceCollection AddConfigurationOption<TAppSettingsOption>(this IServiceCollection services, IConfiguration configuration, string section)
            where TAppSettingsOption : class
        {
            services.AddOptions<TAppSettingsOption>()
                .Bind(configuration.GetSection(section));
            return services;
        }
    }
}
