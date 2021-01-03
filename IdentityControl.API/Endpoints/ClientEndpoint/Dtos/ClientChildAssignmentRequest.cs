using IdentityControl.API.Endpoints.ClientEndpoint.ClientChildren;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientChildAssignmentRequest
    {
        public string Value { get; set; }
        public ClientChildType Type { get; set; }
    }
}