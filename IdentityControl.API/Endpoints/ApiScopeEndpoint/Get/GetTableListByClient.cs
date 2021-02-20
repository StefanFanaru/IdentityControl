using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ApiScopes;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetTableListByClient : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ClientScope> _clientScopeRepository;
        private readonly IApiScopeTableList _tableList;

        public GetTableListByClient(IConfigurationRepository<ClientScope> clientScopeRepository, IApiScopeTableList tableList)
        {
            _clientScopeRepository = clientScopeRepository;
            _tableList = tableList;
        }

        [HttpGet("api-scope/table-list/client/{clientId}")]
        [SwaggerOperation(Summary = "Gets Scopes of a Client in a Table List format",
            Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<PageOf<ApiScopeDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ApiScopeTableList.ApiScopeFilter? filterType, string searchTerm, int clientId,
            CancellationToken cancellationToken = default)
        {
            var apiScopesNames = await _clientScopeRepository.Query()
                .Where(x => x.ClientId == clientId)
                .Select(x => x.Scope)
                .ToListAsync(cancellationToken);

            var query = _tableList.QueryTableList(filterType, searchTerm)
                .Where(x => apiScopesNames.Contains(x.Name));

            return await _tableList.SelectPage(query, pageIndex, pageSize, sortColumn, sortDirection, cancellationToken);
        }
    }
}