namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientScopeDto
    {
        public int Id { get; set; }
        public string Scope { get; set; }
        public int ClientId { get; set; }
    }
}