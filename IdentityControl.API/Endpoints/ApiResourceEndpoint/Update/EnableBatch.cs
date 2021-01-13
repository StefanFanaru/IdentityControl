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
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class EnableBatch : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _repository;

        public EnableBatch(IIdentityRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-resource/enable-batch")]
        [SwaggerOperation(Summary = "Enables multiple API Resources", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopesIds, CancellationToken cancellationToken = default)
        {
            var apiResourcesCount = await _repository.Query()
                .Where(x => apiScopesIds.ToList().Contains(x.Id) &&
                            x.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope)
                .CountAsync(cancellationToken);

            if (apiResourcesCount == 0 || apiResourcesCount < apiScopesIds.Length)
                return NotFound("One ore more instances where not found");

            await _repository.Query()
                .Where(x => apiScopesIds.ToList().Contains(x.Id))
                .ForEachAsync(x =>
                {
                    x.Enabled = true;
                    x.ShowInDiscoveryDocument = true;
                }, cancellationToken);

            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Enabled, null, apiResourcesCount);
            await _repository.SaveAsync(toaster, apiResourcesCount);

            return Ok();
        }
    }
}