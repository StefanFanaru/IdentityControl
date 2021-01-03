using System.Linq;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Endpoints.ClientEndpoint
{
    public static class ClientExtensions
    {
        public static IQueryable<ClientDto> SelectBasicClientDto(this IQueryable<Client> query)
        {
            return query.Select(e => new ClientDto
            {
                Id = e.Id,
                Name = e.ClientId,
                DisplayName = e.ClientName,
                Description = e.Description,
                Enabled = e.Enabled,
                IsReadOnly = e.NonEditable,
                Created = e.Created,
                ClientUri = e.ClientUri,
                AccessTokenLifetime = e.AccessTokenLifetime / 60 // transform seconds in minutes
            });
        }

        public static IQueryable<ClientDto> SelectDetailedClientDto(this IQueryable<Client> query)
        {
            return query.Select(e => new ClientDto
            {
                Id = e.Id,
                Name = e.ClientId,
                DisplayName = e.ClientName,
                Description = e.Description,
                Enabled = e.Enabled,
                IsReadOnly = e.NonEditable,
                Created = e.Created,
                Updated = e.Updated,
                LastAccessed = e.LastAccessed,
                RequirePkce = e.RequirePkce,
                AccessTokenLifetime = e.AccessTokenLifetime / 60, // transform seconds in minutes
                ClientUri = e.ClientUri,
                AllowOfflineAccess = e.AllowOfflineAccess,
                RequireClientSecret = e.RequireClientSecret,
                AllowAccessTokensViaBrowser = e.AllowAccessTokensViaBrowser
                // AllowedScopes = e.AllowedScopes.Select(s => new ClientScopeDto
                // {
                //     Id = s.Id,
                //     Scope = s.Scope
                // }),
                // ClientSecrets = e.ClientSecrets.Select(s => new ClientSecretDto
                // {
                //     Id = s.Id,
                //     Value = s.Value
                // }),
                // RedirectUris = e.RedirectUris.Select(r => new ClientRedirectUriDto
                // {
                //     Id = r.Id,
                //     RedirectUri = r.RedirectUri
                // }),
                // AllowedCorsOrigins = e.AllowedCorsOrigins.Select(c => new ClientCorsOriginDto
                // {
                //     Id = c.Id,
                //     Origin = c.Origin
                // }),
                // AllowedGrantTypes = e.AllowedGrantTypes.Select(g => new ClientGrantTypeDto
                // {
                //     Id = g.Id,
                //     GrantType = g.GrantType
                // }),
                // PostLogoutRedirectUris = e.PostLogoutRedirectUris.Select(p => new ClientPostLogoutRedirectUriDto
                // {
                //     Id = p.Id,
                //     PostLogoutRedirectUri = p.PostLogoutRedirectUri
                // })
            });
        }
    }
}