using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class Disable : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _repository;

        public Disable(IIdentityRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-resource/disable/{id}")]
        [SwaggerOperation(Summary = "Disables an API resource", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken)
        {
            if (!_repository.Query().Any(x => x.Id == id && x.Name != AppConstants.ReadOnlyEntities.IdentityControlApiResource))
                return NotFound($"Instance with ID {id} was not found");

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            entity.Enabled = false;
            entity.ShowInDiscoveryDocument = false;

            _repository.Update(entity);
            var toaster = new ToasterEvent(nameof(ApiResource), ToasterType.Success, ToasterVerbs.Disabled, entity.Name);
            await _repository.SaveAsync(toaster);

            return Ok();
        }
    }
}