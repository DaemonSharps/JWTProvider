using Infrastructure.Common;
using Infrastructure.Constants;
using Infrastructure.DataBase;
using Infrastructure.Middleware;
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
using System.IO;
using System.Text;

namespace JWTProvider
{
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

            services.AddMediatR(typeof(Startup));
            services.AddCors();

            services.AddDbContext<UsersDBContext>(options =>
            options.UseSqlServer(Configuration[ConfigurationKeys.DefaultConnection],
                b => b.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                        ValidateIssuer = true,
                        ValidIssuer = Configuration[ConfigurationKeys.TokenIssuer],
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[ConfigurationKeys.AccessKey])),
                        RoleClaimType = JWTClaimKeys.Role
                    };
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
            {
                options
                .WithOrigins("https://vgarage.vercel.app", "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
