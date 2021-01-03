using System;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Update
{
    public class UpdateClientSecretRequest
    {
        public int OwnerId { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}