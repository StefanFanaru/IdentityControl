using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using IdentityControl.API.Asp;
using IdentityControl.API.Configuration;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using JsonExtensions = IdentityControl.API.Extensions.JsonExtensions;

namespace IdentityControl.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = configuration.GetSection("Settings:Environment").Value;
            StaticConfiguration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfiguration { get; private set; }
        public string Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(IdentityContext).GetTypeInfo().Assembly.GetName().Name;
            var authority = Configuration.GetSection("ApplicationUrls:IdentityServer").Value;

            services
                .AddApiServices()
                .AddAppDatabase(connectionString, migrationsAssembly)
                .AddSwaggerConfiguration()
                .AddAuth();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.AllowInputFormatterExceptionMessages = true;
                options.SerializerSettings.Converters.Add(new JsonExtensions.UtcDateTimeConverter());
                options.SerializerSettings.Converters.Add(new JsonExtensions.TrimmingStringConverter());
            });
            services.AddCors();
            services.AddSignalR();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.Audience = "identity_control";
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerValidator = (issuer, token, parameters) => authority // to support Docker internal network
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];
            app.UsePathBase(pathBase);

            app.UseCors(policy =>
            {
                policy.WithOrigins(
                    Configuration.GetSection("ApplicationUrls:AngularClient").Value);

                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRequestResponseLogging();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.AddSwagger(pathBase);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<EventHub>("/event-hub");
            });
        }
    }
}