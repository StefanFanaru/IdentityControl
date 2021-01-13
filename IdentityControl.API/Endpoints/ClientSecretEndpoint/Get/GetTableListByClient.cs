using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Services.ClientSecrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Services.ClientSecrets.ClientSecretTableList;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class GetTableListByClient : BaseAsyncEndpoint
    {
        private readonly IClientSecretTableList _tableList;

        public GetTableListByClient(IClientSecretTableList tableList)
        {
            _tableList = tableList;
        }

        [HttpGet("client-secret/table-list/client/{clientId}")]
        [SwaggerOperation(Summary = "Gets Secrets of a Client in a Table List format", Tags = new[] {"ClientSecretEndpoint"})]
        public async Task<PageOf<ClientSecretDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ClientSecretFilter? filterType, string searchTerm, int clientId,
            CancellationToken cancellationToken = default)
        {
            var query = _tableList.QueryTableList(filterType, searchTerm).Where(x => x.ClientId == clientId);
            return await _tableList.SelectPage(query, pageIndex, pageSize, sortColumn, sortDirection, cancellationToken);
        }
    }
}