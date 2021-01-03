using System.Collections.Generic;

namespace IdentityControl.API.Asp
{
    public class GetPaginatedResponse<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}