using IdentityControl.API.Endpoints.ClientEndpoint.ClientChildren;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientChildrenDeleteRequest
    {
        public int Id { get; set; }
        public ClientChildType Type { get; set; }
    }
}