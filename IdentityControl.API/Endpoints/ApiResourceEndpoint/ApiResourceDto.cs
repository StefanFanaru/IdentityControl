using System;
using System.Collections.Generic;
using IdentityControl.API.Asp;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint
{
    public class ApiResourceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool IsReadOnly { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<BaseOption<int>> Secrets { get; set; }
        public List<BaseOption<int>> Scopes { get; set; }
    }
}