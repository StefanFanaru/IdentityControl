using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientSecretEndpoint;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Services.ClientSecrets
{
    public class ClientSecretTableList : IClientSecretTableList
    {
        public enum ClientSecretFilter
        {
            Active,
            Expired
        }

        private readonly IConfigurationRepository<ClientSecret> _repository;

        public ClientSecretTableList(IConfigurationRepository<ClientSecret> repository)
        {
            _repository = repository;
        }

        public IQueryable<ClientSecret> QueryTableList(ClientSecretFilter? filterType, string searchTerm)
        {
            var query = filterType switch
            {
                ClientSecretFilter.Active => _repository.Query()
                    .Where(x => x.Expiration == null || x.Expiration > DateTime.UtcNow),
                ClientSecretFilter.Expired => _repository.Query()
                    .Where(x => x.Expiration != null && x.Expiration < DateTime.UtcNow),
                _ => _repository.Query()
            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Description.Contains(searchTerm)
                                         || x.Value.Contains(searchTerm)
                                         || x.Description.Contains(searchTerm)
                                         || x.Client.ClientName.Contains(searchTerm));
            }

            return query;
        }

        public async Task<PageOf<ClientSecretDto>> SelectPage(IQueryable<ClientSecret> query, int pageIndex,
            int pageSize, string sortColumn,
            SortDirection sortDirection, CancellationToken cancellationToken = default)
        {
            return await query
                .Order(sortColumn ?? "Created", sortDirection) // before Select to allow order by Value
                .Select(e => new ClientSecretDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret
                    Type = e.Type,
                    Expiration = e.Expiration,
                    Created = e.Created,
                    ClientName = e.Client.ClientName
                })
                .GetPageAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}