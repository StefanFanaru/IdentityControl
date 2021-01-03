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

namespace IdentityControl.API.Endpoints.ClientEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    public class Delete : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;

        public Delete(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpDelete("client/{id}")]
        [SwaggerOperation(Summary = "Deletes an API scope", Tags = new[] {"ClientEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.Query()
                .Where(e => e.Id == id && e.ClientId != AppConstants.ReadOnlyEntities.AngularClient)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null) return NotFound($"Instance with ID {id} was not found");

            _repository.Delete(entity);
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Info, ToasterVerbs.Deleted, entity.ClientName);
            await _repository.SaveAsync(toaster);

            return NoContent();
        }
    }
}