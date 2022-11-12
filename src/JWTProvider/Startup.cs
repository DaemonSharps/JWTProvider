using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using Infrastructure.DataBase;
using Infrastructure.DataBase.Context;
using Infrastructure.Middleware;
using Infrastructure.Middleware.Options;
using Infrastructure.Middleware.ServiceCollectionExtentions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyCSharp.HttpUserAgentParser.AspNetCore.DependencyInjection;
using MyCSharp.HttpUserAgentParser.MemoryCache.DependencyInjection;

namespace JWTProvider;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddControllers();
        services.AddRouting(ops => ops.LowercaseUrls = true);
        services.AddSwagger();
        services.AddConfigurationOptions(Configuration);

        services
            .AddHttpUserAgentMemoryCachedParser()
            .AddHttpUserAgentParserAccessor();

        services.AddMediatR(typeof(Startup));
        services.AddCors();
#if DEBUG
        services.AddDbContext<UsersDBContext>(options => options.UseInMemoryDatabase("Debug_User_DB"));
#else
        services.AddDbContext<UsersDBContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));
#endif
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var tokenOptions = Configuration.GetOptions<TokenOptions>(TokenOptions.Section);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(cfg => cfg.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                ValidateIssuer = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.AccessKey))
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        var path = Directory.GetCurrentDirectory();
        loggerFactory.AddFile($"{path}\\Logs\\Log.txt");

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
#if DEBUG
        using (var scope = app.ApplicationServices.CreateScope())
        using (var context = scope.ServiceProvider.GetRequiredService<UsersDBContext>())
            context.Database.EnsureCreated();
#endif

        app.UseHttpResponseExceptionMiddleware();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTProvider v1");
            c.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors(options =>
            options
            .WithOrigins(Configuration.GetSection("Cors:Origins").Get<string[]>())
            .AllowAnyHeader()
            .AllowAnyMethod());
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
