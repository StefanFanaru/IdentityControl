using System;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Insert
{
    public class InsertClientSecretRequest
    {
        public string Description { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public int OwnerId { get; set; }
        public DateTime? Expiration { get; set; }
    }
}