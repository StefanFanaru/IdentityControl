using System;
using System.Collections.Generic;
using System.Linq;
using IdentityControl.API.Asp;
using IdentityControl.API.Asp.Authorization;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ApiResourceSecrets;
using IdentityControl.API.Services.ApiScopes;
using IdentityControl.API.Services.ClientSecrets;
using IdentityControl.API.Services.SignalR;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IdentityControl.API.Configuration
{
    public static class ApiServices
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            Log.Information("Adding dependencies to DI");
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo, AspUserInfo>();
            services.AddScoped<ConfigurationStoreOptions>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddScoped<IEventSender, EventSender>();
            services.AddScoped<IAspValidator, AspValidator>();
            services.AddScoped<IApiScopeTableList, ApiScopeTableList>();
            services.AddScoped<IClientSecretTableList, ClientSecretTableList>();
            services.AddScoped<IApiResourceSecretTableList, ApiResourceSecretTableList>();
            services.AddScoped(typeof(IIdentityRepository<>), typeof(IdentityRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddAppDatabase(this IServiceCollection services, string connectionString,
            string migrationsAssembly)
        {
            Log.Information("Adding connections to DB");

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    }));

            services.AddDbContext<ConfigurationDbContext>(options =>
                options.UseSqlServer(connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    }));

            return services;
        }

        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            Dictionary<string, string> GetSecretKeys(string section)
            {
                return Configuration.GetSection(section).Get<Dictionary<string, string>>()
                    .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                    .ToList()
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            var secretKeys = GetSecretKeys("ApiSecretKeys");

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("scope", "identity_control_full")
                    .Build();

                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "identity_control_full");
                });

                options.AddPolicy("AdminOnly",
                    policy => policy
                        .RequireAuthenticatedUser()
                        .RequireClaim("role", "Administrator")
                        .RequireClaim("scope", "identity_control_full"));

                foreach (var secretKey in secretKeys)
                {
                    options.AddPolicy($"{secretKey.Key}-SecretKey",
                        policyBuilder => policyBuilder.AddRequirements(new SecretKeyRequirement(secretKey.Value)));
                }
            });

            services.AddScoped<IAuthorizationHandler, SecretKeyAuthorizationHandler>();

            return services;
        }
    }
}