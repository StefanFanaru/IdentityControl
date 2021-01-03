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

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class DisableBatch : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _repository;

        public DisableBatch(IIdentityRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-scope/disable-batch")]
        [SwaggerOperation(Summary = "Disables multiple API Scopes", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopeIds, CancellationToken cancellationToken = default)
        {
            var apiScopesCount = await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id) &&
                            x.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope)
                .CountAsync(cancellationToken);

            if (apiScopesCount == 0 || apiScopesCount < apiScopeIds.Length)
                return NotFound("One ore more instances where not found");

            await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id))
                .ForEachAsync(x =>
                {
                    x.Enabled = false;
                    x.ShowInDiscoveryDocument = false;
                }, cancellationToken);

            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Disabled, null, apiScopesCount);
            await _repository.SaveAsync(toaster, apiScopesCount);

            return Ok();
        }
    }
}