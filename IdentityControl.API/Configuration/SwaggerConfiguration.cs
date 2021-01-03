using System;
using System.Collections.Generic;
using System.Linq;
using IdentityControl.API.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IdentityControl.API.Configuration
{
    public static class SwaggerConfiguration
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "IdentityControl API", Version = "v1"});
                options.EnableAnnotations();

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:5001/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:5001/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"identity_control_full", "IdentityControl API - full access"}
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }

        public static IApplicationBuilder ConfigureSwaggerUi(this IApplicationBuilder app, string pathBase)
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "IdentityControl.API V1");

                options.OAuthClientId("swagger_ui_identity_control");
                options.OAuthAppName("IdentityControl API - Swagger");
                options.OAuthClientSecret(Configuration["SecretKeys::IdentityControlSwagger"].Stamp());
                options.OAuthUsePkce();
            });

            return app;
        }
    }

    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize =
                context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
                operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                                }
                            }
                        ] = new[] {"identity_control_full"}
                    }
                };
            }
        }
    }
}