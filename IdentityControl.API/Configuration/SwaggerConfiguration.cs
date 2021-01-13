using System;
using System.Collections.Generic;
using System.Linq;
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
            var identityUri = Configuration.GetValue<string>("ApplicationUrls:IdentityAPI");
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("IdentityServer", GetSwaggerInfo("IdentityServer4 - Configuration APIs", "v1",
                    "APIs used for making CRUD operations over the configuration of IdentityServer4"));
                options.SwaggerDoc("Integration", GetSwaggerInfo("IdentityControl - Integration APIs", "v1", string.Empty));

                options.EnableAnnotations();

                options.AddSecurityDefinition("SecretKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter the SecretKey in the text input below.",
                    Name = "SecretKey",
                    Type = SecuritySchemeType.ApiKey
                });

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

                // options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }

        private static OpenApiInfo GetSwaggerInfo(string title, string version, string description)
        {
            var swaggerInfo = new OpenApiInfo()
            {
                Title = title,
                Version = version.ToLower(),
                Description = description
            };
            return swaggerInfo;
        }

        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app, string pathBase)
        {
            var identityControlUri = Configuration.GetValue<string>("ApplicationUrls:IdentityControlAPI");

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{pathBase}/swagger/IdentityServer/swagger.json", "Configuration APIs");
                options.SwaggerEndpoint($"{pathBase}/swagger/Integration/swagger.json", "Integration APIs");
                options.DocumentTitle = "IdentityControl - Swagger";

                options.OAuthClientId("swagger_ui_identity_control");
                options.OAuthAppName("IdentityControl API - Swagger");
                options.OAuth2RedirectUrl($"{identityControlUri}/swagger/oauth2-redirect.html");
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
                    new OpenApiSecurityRequirement()
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
                    },

                    new OpenApiSecurityRequirement
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
                    }
                };
            }
        }
    }
}