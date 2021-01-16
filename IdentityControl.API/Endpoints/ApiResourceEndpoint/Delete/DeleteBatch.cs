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

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class DeleteBatch : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _repository;

        public DeleteBatch(IIdentityRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-resource/delete-batch")]
        [SwaggerOperation(Summary = "Deletes multiple API Resources", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopeIds, CancellationToken cancellationToken = default)
        {
            var apiScopesCount = await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id) &&
                            x.Name != AppConstants.ReadOnlyEntities.IdentityControlApiResource)
                .CountAsync(cancellationToken);

            if (apiScopesCount == 0 || apiScopesCount < apiScopeIds.Length)
            {
                return NotFound("One ore more instances where not found");
            }

            await _repository.Query().Where(x => apiScopeIds.Contains(x.Id)).DeleteAsync(cancellationToken);
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Info, ToasterVerbs.Deleted, null, apiScopesCount);
            await _repository.SaveAsync(toaster, 0);

            return NoContent();
        }
    }
}