using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetOptionsByApiResource : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiScope> _apiScopeRepository;
        private readonly IConfigurationRepository<ApiResourceScope> _clientScopeRepository;
        private readonly IConfigurationRepository<IdentityResource> _configurationResourceRepo;

        public GetOptionsByApiResource(IConfigurationRepository<ApiResourceScope> clientScopeRepository,
            IConfigurationRepository<ApiScope> apiScopeRepository,
            IConfigurationRepository<IdentityResource> configurationResourceRepo)
        {
            _clientScopeRepository = clientScopeRepository;
            _apiScopeRepository = apiScopeRepository;
            _configurationResourceRepo = configurationResourceRepo;
        }

        [HttpGet("api-scope/api-resource/{apiResourceId}/options")]
        [SwaggerOperation(Summary = "Gets all the scopes of a client in as options", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<List<BaseOption<string>>> HandleAsync(int apiResourceId, CancellationToken cancellationToken = default)
        {
            var names = await _clientScopeRepository.Query()
                .Where(x => x.ApiResourceId == apiResourceId)
                .Select(x => x.Scope)
                .ToListAsync(cancellationToken);

            var apiScopes = await _apiScopeRepository.Query()
                .Where(x => names.Contains(x.Name))
                .Select(e => new BaseOption<string>
                {
                    Value = e.Name,
                    Text = e.DisplayName
                }).ToListAsync(cancellationToken);

            // Including identity resources because they are requested as a scope
            apiScopes.AddRange(_configurationResourceRepo.Query()
                .Where(x => names.Contains(x.Name))
                .Select(x => new BaseOption<string>
                {
                    Value = x.Name,
                    Text = x.DisplayName
                }));

            return apiScopes;
        }
    }
}