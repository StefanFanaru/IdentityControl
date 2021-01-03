using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Endpoints.ClientSecretEndpoint;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Services.ClientSecrets
{
    public interface IClientSecretTableList
    {
        IQueryable<ClientSecret> QueryTableList(ClientSecretTableList.ClientSecretFilter? filterType, string searchTerm);

        Task<PageOf<ClientSecretDto>> SelectPage(IQueryable<ClientSecret> query, int pageIndex,
            int pageSize, string sortColumn,
            SortDirection sortDirection, CancellationToken cancellationToken = default);
    }
}