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
    public class GetApiScopeOptions : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _apiScopeRepository;
        private readonly IIdentityRepository<IdentityResource> _identityResourceRepo;

        public GetApiScopeOptions(IIdentityRepository<ApiScope> apiScopeRepository,
            IIdentityRepository<IdentityResource> identityResourceRepo)
        {
            _apiScopeRepository = apiScopeRepository;
            _identityResourceRepo = identityResourceRepo;
        }

        [HttpGet("api-scope/options")]
        [SwaggerOperation(Summary = "Gets all possible API Scopes options for selection", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<List<BaseOption<string>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var apiScopes = await _apiScopeRepository.Query()
                .Select(e => new BaseOption<string>
                {
                    Value = e.Name,
                    Text = e.DisplayName
                }).ToListAsync(cancellationToken);

            // Including identity resources because they are requested as a scope
            apiScopes.AddRange(_identityResourceRepo.Query().Select(e => new BaseOption<string>
            {
                Value = e.Name,
                Text = e.DisplayName
            }));

            return apiScopes;
        }
    }
}