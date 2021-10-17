using Infrastructure.Common;
using Infrastructure.CustomAttributes.Swagger;
using Infrastructure.DataBase;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<CommandAttributeFilter>();
                c.OperationFilter<QuerryAttributeFilter>();
                c.EnableAnnotations();
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "JWTProvider",
                        Version = "v1",
                        Description = "Authorization provider for [DaemonSharps](https://github.com/DaemonSharps) apps"
                    });
            });

            services.AddMediatR(typeof(Startup));

            services.AddDbContext<UsersDBContext>(options => options.UseSqlServer(Configuration[ConfigurationKeys.DefaultConnection]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTProvider v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger();
            });
        }
    }
}
