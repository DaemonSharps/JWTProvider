using Infrastructure.DataBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JWTProviderIntegration.Common
{
    public class JWTProviderTestFixture<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<TStartup>();
                })
                .ConfigureAppConfiguration((cont, conf)
                    => conf.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false));
            return builder;
        }
    }
}

