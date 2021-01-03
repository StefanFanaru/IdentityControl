namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientRedirectUriDto
    {
        public int Id { get; set; }
        public string RedirectUri { get; set; }
        public int ClientId { get; set; }
    }
}