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

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    public class Delete : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _repository;

        public Delete(IIdentityRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpDelete("api-scope/{id}")]
        [SwaggerOperation(Summary = "Deletes an API scope", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.Query()
                .Where(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null) return NotFound($"Instance with ID {id} was not found");

            _repository.Delete(entity);
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Info, ToasterVerbs.Deleted, entity.Name);
            await _repository.SaveAsync(toaster);

            return NoContent();
        }
    }
}