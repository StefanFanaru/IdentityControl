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
    public class Get : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResourceSecret> _secretRepository;

        public Get(IConfigurationRepository<ApiResourceSecret> secretRepository)
        {
            _secretRepository = secretRepository;
        }

        [HttpGet("api-resource-secret/{id}")]
        [SwaggerOperation(Summary = "Retrieves a API Resource Secret", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<ActionResult<ApiResourceSecretDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _secretRepository.Query()
                .Where(e => e.Id == id)
                .Select(e => new ApiResourceSecretDto
                {
                    Id = e.Id,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret,
                    Description = e.Description,
                    Type = e.Type.ToString(),
                    Expiration = e.Expiration,
                    Created = e.Created,
                    ClientId = e.ApiResource.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (response == null) return NotFound($"Instance with ID {id} was not found");

            return response;
        }
    }
}