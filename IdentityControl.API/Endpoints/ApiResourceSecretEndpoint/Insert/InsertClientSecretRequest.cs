using System;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Insert
{
    public class InsertApiResourceSecretRequest
    {
        public string Description { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public int OwnerId { get; set; }
        public DateTime? Expiration { get; set; }
    }
}