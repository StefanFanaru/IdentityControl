using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    public class GetTableList : BaseAsyncEndpoint
    {
        public enum ClientFilter
        {
            Enabled,
            Disabled,
            PkceOnly,
            WithOfflineAccess,
            WithClientSecret,
            WithBrowserAccessTokens
        }

        private readonly IIdentityRepository<Client> _repository;

        public GetTableList(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpGet("client/table-list")]
        [SwaggerOperation(Summary = "Gets Clients in a Table List format", Tags = new[] {"ClientEndpoint"})]
        public async Task<PageOf<ClientDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ClientFilter? filterType, string searchTerm,
            CancellationToken cancellationToken = default)
        {
            var query = filterType switch
            {
                ClientFilter.Enabled => _repository.Query().Where(x => x.Enabled),
                ClientFilter.Disabled => _repository.Query().Where(x => !x.Enabled),
                ClientFilter.PkceOnly => _repository.Query().Where(x => x.RequirePkce),
                ClientFilter.WithOfflineAccess => _repository.Query().Where(x => x.AllowOfflineAccess),
                ClientFilter.WithClientSecret => _repository.Query().Where(x => x.ClientSecrets.Any()),
                ClientFilter.WithBrowserAccessTokens => _repository.Query().Where(x => x.AllowAccessTokensViaBrowser),
                _ => _repository.Query()
            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Description.Contains(searchTerm)
                                         || x.ClientId.Contains(searchTerm)
                                         || x.ClientUri.Contains(searchTerm)
                                         || x.Description.Contains(searchTerm)
                                         || x.ClientName.Contains(searchTerm));
            }

            return await query.SelectBasicClientDto()
                .Order(sortColumn ?? "DisplayName", sortDirection)
                .GetPageAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}