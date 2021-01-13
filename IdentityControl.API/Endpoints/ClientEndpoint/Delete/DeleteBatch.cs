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
using Z.EntityFramework.Plus;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class DeleteBatch : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;

        public DeleteBatch(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpPatch("client/delete-batch")]
        [SwaggerOperation(Summary = "Deletes multiple Clients", Tags = new[] {"ClientEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopeIds, CancellationToken cancellationToken = default)
        {
            var clientsCount = await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id) && x.ClientId != AppConstants.ReadOnlyEntities.AngularClient)
                .CountAsync(cancellationToken);

            if (clientsCount == 0 || clientsCount < apiScopeIds.Length)
            {
                return NotFound("One ore more instances where not found");
            }

            await _repository.Query().Where(x => apiScopeIds.Contains(x.Id)).DeleteAsync(cancellationToken);
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Info, ToasterVerbs.Deleted, null, clientsCount);
            await _repository.SaveAsync(toaster, 0);

            return NoContent();
        }
    }
}