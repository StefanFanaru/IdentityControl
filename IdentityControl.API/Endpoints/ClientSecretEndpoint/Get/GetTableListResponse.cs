using System.Collections.Generic;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Get
{
    public class GetTableListResponse<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}