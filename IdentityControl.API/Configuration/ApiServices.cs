using System;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ApiResourceSecrets;
using IdentityControl.API.Services.ApiScopes;
using IdentityControl.API.Services.ClientSecrets;
using IdentityControl.API.Services.SignalR;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IdentityControl.API.Configuration
{
    public static class ApiServices
    {
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
    }
}