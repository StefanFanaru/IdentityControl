using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Delete : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResourceSecret> _repository;

        public Delete(IIdentityRepository<ApiResourceSecret> repository)
        {
            _repository = repository;
        }

        [HttpDelete("api-resource-secret/{id}")]
        [SwaggerOperation(Summary = "Deletes a new API Resource Secret", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.Query()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null)
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            _repository.Delete(entity);
            var toaster = new ToasterEvent(nameof(ApiResourceSecret), ToasterType.Info, ToasterVerbs.Deleted);
            await _repository.SaveAsync(toaster);

            return NoContent();
        }
    }
}