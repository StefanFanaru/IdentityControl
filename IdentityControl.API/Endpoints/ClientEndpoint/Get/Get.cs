using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Get : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<Client> _repository;

        public Get(IConfigurationRepository<Client> repository)
        {
            _repository = repository;
        }


        [HttpGet("client/{id}")]
        [SwaggerOperation(Summary = "Retrieves a Client", Tags = new[] {"ClientEndpoint"})]
        public async Task<ActionResult<ClientDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _repository.Query()
                .Where(e => e.Id == id)
                .SelectDetailedClientDto()
                .FirstOrDefaultAsync(cancellationToken);

            if (response == null)
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            return response;
        }
    }
}