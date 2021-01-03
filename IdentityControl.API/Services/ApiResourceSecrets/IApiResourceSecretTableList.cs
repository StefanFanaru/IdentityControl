using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Endpoints.ApiResourceSecretEndpoint;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Services.ApiResourceSecrets
{
    public interface IApiResourceSecretTableList
    {
        IQueryable<ApiResourceSecret> QueryTableList(ApiResourceSecretTableList.ApiResourceSecretFilter? filterType,
            string searchTerm);

        Task<PageOf<ApiResourceSecretDto>> SelectPage(IQueryable<ApiResourceSecret> query, int pageIndex, int pageSize,
            string sortColumn,
            SortDirection sortDirection, CancellationToken cancellationToken = default);
    }
}