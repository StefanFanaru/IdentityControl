namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Update
{
    public class UpdateApiResourceRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string[] ApiScopes { get; set; }
    }
}