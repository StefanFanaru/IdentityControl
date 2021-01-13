using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class GetAll : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ClientSecret> _repository;

        public GetAll(IIdentityRepository<ClientSecret> repository)
        {
            _repository = repository;
        }

        [HttpGet("client-secret/all")]
        [SwaggerOperation(Summary = "Gets all the scopes", Tags = new[] {"ClientSecretEndpoint"})]
        public async Task<List<ClientSecretDto>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .Select(e => new ClientSecretDto
                {
                    Id = e.Id,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret
                    Type = e.Type.ToString(),
                    Expiration = e.Expiration,
                    Created = e.Created,
                    Description = e.Description,
                    ClientName = e.Client.ClientName
                }).ToListAsync(cancellationToken);
        }
    }
}