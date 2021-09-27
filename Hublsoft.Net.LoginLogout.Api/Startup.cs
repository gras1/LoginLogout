using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace Hublsoft.Net.LoginLogout.Api
{
    #pragma warning disable 1591
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

            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));
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

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    #pragma warning restore 1591
}
