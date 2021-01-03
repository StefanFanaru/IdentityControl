namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientPostLogoutRedirectUriDto
    {
        public int Id { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public int ClientId { get; set; }
    }
}