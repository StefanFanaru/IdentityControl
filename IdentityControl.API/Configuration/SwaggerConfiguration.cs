using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IdentityControl.API.Configuration
{
    public static class SwaggerConfiguration
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            var identityUri = Configuration.GetValue<string>("ApplicationUrls:IdentityServer");
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("Internal", GetSwaggerInfo("IdentityServer4 - Internal APIs", "v1",
                    "APIs used for making CRUD operations over the configuration of IdentityServer4"));
                options.SwaggerDoc("Integration", GetSwaggerInfo("IdentityControl - Integration APIs", "v1", string.Empty));

                options.EnableAnnotations();

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{identityUri}/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"identity_control_full", "IdentityControl API - full access"}
                            }
                        }
                    }
                });

                options.AddSecurityDefinition("SecretKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter the SecretKey in the text input below.",
                    Name = "SecretKey",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] {"identity_control_full"} // api scope
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "SecretKey",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "SecretKey"
                            },
                        },
                        new[] {"identity_control_full"}
                    }
                });
            });

            return services;
        }

        private static OpenApiInfo GetSwaggerInfo(string title, string version, string description)
        {
            return new OpenApiInfo()
            {
                Title = title,
                Version = version.ToLower(),
                Description = description
            };
        }

        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app, string pathBase)
        {
            var apiUri = Configuration.GetValue<string>("ApplicationUrls:IdentityControl");

            app.UseSwagger(options =>
            {
                if (!string.IsNullOrEmpty(pathBase))
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer>
                            {new OpenApiServer {Url = $"{apiUri}{pathBase}"}};
                    });
                }
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{pathBase}/swagger/Internal/swagger.json", "Internal APIs");
                options.SwaggerEndpoint($"{pathBase}/swagger/Integration/swagger.json", "Integration APIs");
                options.DocumentTitle = "IdentityControl - Swagger";

                options.OAuthClientId("swagger_ui_identity_control");
                options.OAuthAppName("IdentityControl API - Swagger");
                options.OAuth2RedirectUrl($"{apiUri}{pathBase}/swagger/oauth2-redirect.html");
            });

            return app;
        }
    }
}
