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

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class EnableBatch : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiScope> _repository;

        public EnableBatch(IConfigurationRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpPatch("api-scope/enable-batch")]
        [SwaggerOperation(Summary = "Enables multiple API Scopes", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] apiScopesIds, CancellationToken cancellationToken = default)
        {
            var apiScopesCount = await _repository.Query()
                .Where(x => apiScopesIds.ToList().Contains(x.Id) &&
                            x.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope)
                .CountAsync(cancellationToken);

            if (apiScopesCount == 0 || apiScopesCount < apiScopesIds.Length)
            {
                return NotFound("One ore more instances where not found");
            }

            await _repository.Query()
                .Where(x => apiScopesIds.ToList().Contains(x.Id))
                .ForEachAsync(x =>
                {
                    x.Enabled = true;
                    x.ShowInDiscoveryDocument = true;
                }, cancellationToken);

            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Enabled, null, apiScopesCount);
            await _repository.SaveAsync(toaster, apiScopesCount);

            return Ok();
        }
    }
}