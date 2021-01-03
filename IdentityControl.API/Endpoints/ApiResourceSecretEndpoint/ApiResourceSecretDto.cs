using System;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint
{
    public class ApiResourceSecretDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }

        public string Value { get; set; }
        public string Type { get; set; }
        public string ApiResourceName { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Expiration { get; set; }
    }
}