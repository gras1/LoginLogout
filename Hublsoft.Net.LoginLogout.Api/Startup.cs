using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Reflection;
using System.IO;
using Hublsoft.Net.LoginLogout.DataAccess;
using Hublsoft.Net.LoginLogout.Bll;
using System.Diagnostics.CodeAnalysis;

namespace Hublsoft.Net.LoginLogout.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiVersioning(config =>
            {
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.ReportApiVersions = true;
            });
            services.AddSwaggerGen(c => {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.ExampleFilters();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hublsoft.Net.LoginLogout.Api", Version = "v1" });
            });
            services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

            services.Configure<DatabaseOptions>(Configuration.GetSection("DatabaseOptions"));
            services.AddScoped<IRegisteredUsersRepository, RegisteredUsersRepository>();
            services.AddScoped<IRegisteredUserAuditsRepository, RegisteredUserAuditsRepository>();
            services.AddScoped<IUserManager, UserManager>();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "Open",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader());
            }); 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hublsoft.Net.LoginLogout.Api v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("Open");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
