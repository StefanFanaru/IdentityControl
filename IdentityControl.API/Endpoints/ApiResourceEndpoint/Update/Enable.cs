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

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Enable : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResource> _repository;

        public Enable(IConfigurationRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-resource/enable/{id}")]
        [SwaggerOperation(Summary = "Enables an API resource", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken)
        {
            if (!_repository.Query().Any(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiResource))
                return NotFound($"Instance with ID {id} was not found");

            var entity = await _repository.Query()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            entity.Enabled = true;
            entity.ShowInDiscoveryDocument = true;

            var toaster = new ToasterEvent(nameof(ApiResource), ToasterType.Success, ToasterVerbs.Enabled, entity.Name);
            await _repository.SaveAsync(toaster);

            return Ok();
        }
    }
}