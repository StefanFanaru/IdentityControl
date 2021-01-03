using System;
using System.Collections.Generic;
using IdentityControl.API.Endpoints.ClientSecretEndpoint;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public string ClientUri { get; set; }
        public bool Enabled { get; set; }
        public bool RequirePkce { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public string Description { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public int AccessTokenLifetime { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }

        public IEnumerable<ClientSecretDto> ClientSecrets { get; set; }
        public IEnumerable<ClientGrantTypeDto> AllowedGrantTypes { get; set; }
        public IEnumerable<ClientRedirectUriDto> RedirectUris { get; set; }
        public IEnumerable<ClientPostLogoutRedirectUriDto> PostLogoutRedirectUris { get; set; }
        public IEnumerable<ClientScopeDto> AllowedScopes { get; set; }
        public IEnumerable<ClientCorsOriginDto> AllowedCorsOrigins { get; set; }
    }
}