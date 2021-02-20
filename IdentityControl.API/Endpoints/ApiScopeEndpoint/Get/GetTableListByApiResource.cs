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
    public class GetTableListByApiResource : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResourceScope> _apiResourceScopeRepository;
        private readonly IApiScopeTableList _tableList;

        public GetTableListByApiResource(IConfigurationRepository<ApiResourceScope> apiResourceScopeRepository,
            IApiScopeTableList tableList)
        {
            _apiResourceScopeRepository = apiResourceScopeRepository;
            _tableList = tableList;
        }

        [HttpGet("api-scope/table-list/api-resource/{apiResourceId}")]
        [SwaggerOperation(Summary = "Gets the Scopes of an API Resource in a Table List format",
            Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<PageOf<ApiScopeDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ApiScopeTableList.ApiScopeFilter? filterType, string searchTerm, int apiResourceId,
            CancellationToken cancellationToken = default)
        {
            var apiScopesNames = await _apiResourceScopeRepository.Query()
                .Where(x => x.ApiResourceId == apiResourceId)
                .Select(x => x.Scope)
                .ToArrayAsync(cancellationToken);

            var query = _tableList.QueryTableList(filterType, searchTerm)
                .Where(x => apiScopesNames.Contains(x.Name));

            return await _tableList.SelectPage(query, pageIndex, pageSize, sortColumn, sortDirection, cancellationToken);
        }
    }
}