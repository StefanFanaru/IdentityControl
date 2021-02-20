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
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Get : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ClientSecret> _secretRepository;

        public Get(IConfigurationRepository<ClientSecret> secretRepository)
        {
            _secretRepository = secretRepository;
        }

        [HttpGet("client-secret/{id}")]
        [SwaggerOperation(Summary = "Retrieves a Client secret", Tags = new[] {"ClientSecretEndpoint"})]
        public async Task<ActionResult<ClientSecretDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _secretRepository.Query()
                .Where(e => e.Id == id)
                .Select(e => new ClientSecretDto
                {
                    Id = e.Id,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret
                    Description = e.Description,
                    Type = e.Type.ToString(),
                    Expiration = e.Expiration,
                    Created = e.Created,
                    ClientName = e.Client.ClientName
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (response == null)
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            return response;
        }
    }
}