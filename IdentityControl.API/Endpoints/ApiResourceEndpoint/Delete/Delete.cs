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

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Delete : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _repository;

        public Delete(IIdentityRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpDelete("api-resource/{id}")]
        [SwaggerOperation(Summary = "Deletes an API resource", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.Query()
                .Where(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiResource)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            _repository.Delete(entity);
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Info, ToasterVerbs.Deleted, entity.Name);
            await _repository.SaveAsync(toaster);

            return NoContent();
        }
    }
}