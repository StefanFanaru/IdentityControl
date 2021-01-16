using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Services.ApiResourceSecrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Services.ApiResourceSecrets.ApiResourceSecretTableList;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetTableListByApiResource : BaseAsyncEndpoint
    {
        private readonly IApiResourceSecretTableList _tableList;

        public GetTableListByApiResource(IApiResourceSecretTableList tableList)
        {
            _tableList = tableList;
        }

        [HttpGet("api-resource-secret/table-list/api-resource/{apiResourceId}")]
        [SwaggerOperation(Summary = "Gets Secrets of an API Resource in a Table List format",
            Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<PageOf<ApiResourceSecretDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ApiResourceSecretFilter? filterType, string searchTerm, int apiResourceId,
            CancellationToken cancellationToken = default)
        {
            var query = _tableList.QueryTableList(filterType, searchTerm)
                .Where(x => x.ApiResourceId == apiResourceId);
            return await _tableList.SelectPage(query, pageIndex, pageSize, sortColumn, sortDirection, cancellationToken);
        }
    }
}