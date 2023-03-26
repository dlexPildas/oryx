using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using OryxDomain.Models;
using OryxDomain.Utilities;
using OryxGateway.Services;
using System;
using System.Text;

namespace OryxGateway
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"Ocelot.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            Environment = env;
        }

        public IConfiguration Configuration { get; }
        readonly IWebHostEnvironment Environment;
        readonly string MyAllowSpecificOrigins = "MyAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    if (Environment.IsDevelopment() || Environment.IsStaging())
                    {
                        builder
                            .WithOrigins(new string[]
                            {
                                "http://localhost:8089",
                                "http://192.168.1.12:8089",
                                "https://localhost:8080",
                                "https://192.168.1.12:8080",
                            })
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                    else
                    {
                        string[] listIPValid = Configuration.GetSection("listIPValid").Get<string[]>();

                        builder
                            .WithOrigins(listIPValid)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                    builder.AllowCredentials();
                });
            });
            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            var key = Encoding.ASCII.GetBytes(Configuration["Jwtkey"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddControllers();
            SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            services.AddOcelot(Configuration)
                .AddPolly();

            Parameters.InitParameters(Configuration["OryxPath"] + "oryx.ini");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var middleware = new OcelotPipelineConfiguration
            {
                PreQueryStringBuilderMiddleware = async (ctx, next) =>
                {
                    SecurityService securityService = new SecurityService(Configuration);
                    await securityService.ProcessMiddleware(ctx, next);
                }
            };
            if (env.IsDevelopment() || env.IsStaging() || env.IsEnvironment("Staging2"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.All
            });

            if (env.IsDevelopment())
            {
                RewriteOptions options = new RewriteOptions()
                    .AddRedirectToHttpsPermanent();
                app.UseRewriter(options);

                app.UseHttpsRedirection();
            }
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseMvc();
            app.UseOcelot(middleware).Wait();
        }
    }
}
