﻿using System.Linq;
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
    [ApiExplorerSettings(GroupName = "Internal")]
    public class DisableBatch : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<Client> _repository;

        public DisableBatch(IConfigurationRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpPatch("client/disable-batch")]
        [SwaggerOperation(Summary = "Disables multiple Clients", Tags = new[] {"ClientEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopeIds, CancellationToken cancellationToken = default)
        {
            var clientsCount = await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id) && x.ClientId != AppConstants.ReadOnlyEntities.AngularClient)
                .CountAsync(cancellationToken);

            if (clientsCount == 0 || clientsCount < apiScopeIds.Length)
            {
                return NotFound("One ore more instances where not found");
            }

            await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id))
                .ForEachAsync(x => x.Enabled = false, cancellationToken);

            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Disabled, null, clientsCount);
            await _repository.SaveAsync(toaster, clientsCount);

            return Ok();
        }
    }
}