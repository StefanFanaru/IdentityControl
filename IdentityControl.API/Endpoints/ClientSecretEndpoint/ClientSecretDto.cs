using System;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint
{
    public class ClientSecretDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ClientName { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expiration { get; set; }
    }
}