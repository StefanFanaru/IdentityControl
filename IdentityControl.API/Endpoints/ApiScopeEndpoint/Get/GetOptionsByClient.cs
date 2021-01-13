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
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class GetOptionsByClient : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _apiScopeRepository;
        private readonly IIdentityRepository<ClientScope> _clientScopeRepository;
        private readonly IIdentityRepository<IdentityResource> _identityResourceRepo;

        public GetOptionsByClient(IIdentityRepository<ClientScope> clientScopeRepository,
            IIdentityRepository<ApiScope> apiScopeRepository,
            IIdentityRepository<IdentityResource> identityResourceRepo)
        {
            _clientScopeRepository = clientScopeRepository;
            _apiScopeRepository = apiScopeRepository;
            _identityResourceRepo = identityResourceRepo;
        }

        [HttpGet("api-scope/client/{clientId}/options")]
        [SwaggerOperation(Summary = "Gets all the scopes of a client in as options", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<List<BaseOption<string>>> HandleAsync(int clientId, CancellationToken cancellationToken = default)
        {
            var names = await _clientScopeRepository.Query()
                .Where(x => x.ClientId == clientId)
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
            apiScopes.AddRange(_identityResourceRepo.Query()
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