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

namespace IdentityControl.API.Endpoints.ClientEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class Disable : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;

        public Disable(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpPatch("client/disable/{id}")]
        [SwaggerOperation(Summary = "Disables a Client", Tags = new[] {"ClientEndpoint"})]
        public async Task<IActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_repository.Query().Any(x => x.Id == id && x.ClientId != AppConstants.ReadOnlyEntities.AngularClient))
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            entity.Enabled = false;

            _repository.Update(entity);
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Disabled, entity.ClientName);
            await _repository.SaveAsync(toaster);

            return Ok();
        }
    }
}