using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/*
 * TO MAKE IT WORK WITH NETCORE 2.0
 * 
 * ISSUE: SqlClient fails with netcoreapp2.0 on Win7/Server2008
 * TO SOLVE: Using dapper 1.50.2, which has a dependency on SqlClient 4.1, caused this issue. Explicitly installing SqlClient 4.4 indeed solved the issue.
 ***/

namespace APS.Web.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                        .AllowAnyMethod()
                                                                        .AllowAnyHeader()
                                                                        .AllowCredentials()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            Models.AppSettings.ConnectionString_Api = Configuration.GetValue<string>("ConnectionString");
            Models.AppSettings.ConnectionString_Docs = Configuration.GetValue<string>("ConnectionStrings:DocsConnectionString");
            Models.AppSettings.ConnectionString_Reclamos = Configuration.GetValue<string>("ConnectionStrings:ReclamosConnectionString");
            Models.AppSettings.ConnectionString_Correspondencia = Configuration.GetValue<string>("ConnectionStrings:CorrespondenciaConnectionString");

            Models.AppSettings.ConnectionString_Seguros = Configuration.GetValue<string>("ConnectionStrings:SegurosConnectionString");
            Models.AppSettings.ConnectionString_Sipof = Configuration.GetValue<string>("ConnectionStrings:SipofConnectionString");
            Models.AppSettings.ConnectionString_Web = Configuration.GetValue<string>("ConnectionStrings:WebConnectionString");

            Models.AppSettings.ResolucionesPath = Configuration.GetValue<string>("ResolucionesPath");
            Models.AppSettings.CircularesPath = Configuration.GetValue<string>("CircularesPath");

            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
