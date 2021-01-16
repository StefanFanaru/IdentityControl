using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Services.ApiScopes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetTableList : BaseAsyncEndpoint
    {
        private readonly IApiScopeTableList _tableList;

        public GetTableList(IApiScopeTableList tableList)
        {
            _tableList = tableList;
        }

        [HttpGet("api-scope/table-list")]
        [SwaggerOperation(Summary = "Gets all API Scopes in a Table List format", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<PageOf<ApiScopeDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ApiScopeTableList.ApiScopeFilter? filterType, string searchTerm,
            CancellationToken cancellationToken = default)
        {
            var query = _tableList.QueryTableList(filterType, searchTerm);
            return await _tableList.SelectPage(query, pageIndex, pageSize, sortColumn, sortDirection, cancellationToken);
        }
    }
}