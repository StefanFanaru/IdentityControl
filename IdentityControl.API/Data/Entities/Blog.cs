using System.Collections.Generic;

namespace IdentityControl.API.Data.Entities
{
    public class Blog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public List<BlogOwner> Owners { get; set; }
    }
}