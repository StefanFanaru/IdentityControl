﻿namespace IdentityControl.API.Endpoints.ClientEndpoint.Dtos
{
    public class ClientCorsOriginDto
    {
        public int Id { get; set; }
        public string Origin { get; set; }
        public int ClientId { get; set; }
    }
}