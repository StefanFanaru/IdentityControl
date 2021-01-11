namespace IdentityControl.API.Endpoints.ClientEndpoint.Insert
{
    public class InsertClientRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsReadOnly { get; set; }
        public bool RequirePkce { get; set; }
        public int AccessTokenLifetime { get; set; }
        public string ClientUri { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool RequireClientSecret { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public string[] ApiScopes { get; set; }
    }
}