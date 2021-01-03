namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Update
{
    public class RegenerateClientSecretRequest
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}