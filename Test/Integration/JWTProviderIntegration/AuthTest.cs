using JWTProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Infrastructure.Middleware;
using System.Net.Http.Json;
using JWTProvider.User.Commands;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Token;

public class TestFixture : WebApplicationFactory<Startup>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webHostBuilder =>
            {
                webHostBuilder.UseStartup<Startup>();
                webHostBuilder.ConfigureTestServices(c =>
                {
                    var descr = c.Single(s => s.ServiceType == typeof(DbContextOptions<UsersDBContext>));
                    c.Remove(descr);
                    c.AddDbContext<UsersDBContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
                });
            })
            .ConfigureAppConfiguration((cont, conf)
                => conf.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false));
        return builder;
    }

}

public class AuthTest : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;

    public AuthTest(TestFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Test()
    {
        var content = JsonContent.Create(new UserRegistrationCommand
        {
            Email = "tes@mail.ru",
            Password = "test",
            FirstName = "1"
        });
       var result = await _client.PostAsync("/user", content);  
       result.EnsureSuccessStatusCode();
       Assert.NotNull(result);
    }
}

