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

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class DisableBatch : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResource> _repository;

        public DisableBatch(IConfigurationRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-resource/disable-batch")]
        [SwaggerOperation(Summary = "Disables multiple API Scopes", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopeIds, CancellationToken cancellationToken = default)
        {
            var apiResourcesCount = await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id) &&
                            x.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope)
                .CountAsync(cancellationToken);

            if (apiResourcesCount == 0 || apiResourcesCount < apiScopeIds.Length)
                return NotFound("One ore more instances where not found");

            await _repository.Query()
                .Where(x => apiScopeIds.ToList().Contains(x.Id))
                .ForEachAsync(x =>
                {
                    x.Enabled = false;
                    x.ShowInDiscoveryDocument = false;
                }, cancellationToken);

            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Disabled, null, apiResourcesCount);
            await _repository.SaveAsync(toaster, apiResourcesCount);

            return Ok();
        }
    }
}