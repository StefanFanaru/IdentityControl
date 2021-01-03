namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Insert
{
    public class InsertApiResourceRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string[] ApiScopes { get; set; }
    }
}