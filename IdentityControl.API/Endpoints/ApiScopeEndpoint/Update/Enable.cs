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

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Enable : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _repository;

        public Enable(IIdentityRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-scope/enable/{id}")]
        [SwaggerOperation(Summary = "Enables an API scope", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_repository.Query().Any(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope))
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            entity.Enabled = true;
            entity.ShowInDiscoveryDocument = true;

            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Enabled, entity.Name);
            await _repository.SaveAsync(toaster);

            return Ok();
        }
    }
}