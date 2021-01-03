using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Endpoints.ApiScopeEndpoint;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Services.ApiScopes
{
    public interface IApiScopeTableList
    {
        IQueryable<ApiScope> QueryTableList(ApiScopeTableList.ApiScopeFilter? filterType, string searchTerm);

        Task<PageOf<ApiScopeDto>> SelectPage(IQueryable<ApiScope> query, int pageIndex,
            int pageSize, string sortColumn,
            SortDirection sortDirection, CancellationToken cancellationToken = default);
    }
}