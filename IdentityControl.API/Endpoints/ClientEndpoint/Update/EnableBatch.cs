using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class EnableBatch : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;

        public EnableBatch(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpPatch("client/enable-batch")]
        [SwaggerOperation(Summary = "Enables multiple Clients", Tags = new[] {"ClientEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopesIds, CancellationToken cancellationToken = default)
        {
            var clientsCount = await _repository.Query()
                .Where(x => apiScopesIds.ToList().Contains(x.Id) && x.ClientId != AppConstants.ReadOnlyEntities.AngularClient)
                .CountAsync(cancellationToken);

            if (clientsCount == 0 || clientsCount < apiScopesIds.Length)
            {
                return NotFound("One ore more instances where not found");
            }

            await _repository.Query()
                .Where(x => apiScopesIds.ToList().Contains(x.Id))
                .ForEachAsync(x => x.Enabled = true, cancellationToken);

            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Enabled, null, clientsCount);
            await _repository.SaveAsync(toaster, clientsCount);

            return Ok();
        }
    }
}