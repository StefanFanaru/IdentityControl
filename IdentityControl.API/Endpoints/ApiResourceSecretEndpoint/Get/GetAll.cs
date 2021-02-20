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

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetAll : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResourceSecret> _repository;

        public GetAll(IConfigurationRepository<ApiResourceSecret> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-resource-secret/all")]
        [SwaggerOperation(Summary = "Gets all the scopes", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<List<ApiResourceSecretDto>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .Select(e => new ApiResourceSecretDto
                {
                    Id = e.Id,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret,
                    Type = e.Type.ToString(),
                    Expiration = e.Expiration,
                    Created = e.Created,
                    Description = e.Description,
                    ClientId = e.ApiResource.Name
                }).ToListAsync(cancellationToken);
        }
    }
}