using System;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Update
{
    public class UpdateApiResourceSecretRequest
    {
        public int OwnerId { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}