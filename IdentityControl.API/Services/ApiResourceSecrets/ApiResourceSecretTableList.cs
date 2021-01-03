using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ApiResourceSecretEndpoint;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Services.ApiResourceSecrets
{
    public class ApiResourceSecretTableList : IApiResourceSecretTableList
    {
        public enum ApiResourceSecretFilter
        {
            Active,
            Expired
        }

        private readonly IIdentityRepository<ApiResourceSecret> _repository;

        public ApiResourceSecretTableList(IIdentityRepository<ApiResourceSecret> repository)
        {
            _repository = repository;
        }

        public IQueryable<ApiResourceSecret> QueryTableList(ApiResourceSecretFilter? filterType, string searchTerm)
        {
            var query = filterType switch
            {
                ApiResourceSecretFilter.Active => _repository.Query()
                    .Where(x => x.Expiration == null || x.Expiration > DateTime.UtcNow),
                ApiResourceSecretFilter.Expired => _repository.Query()
                    .Where(x => x.Expiration != null && x.Expiration < DateTime.UtcNow),
                _ => _repository.Query()
            };

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.Description.Contains(searchTerm)
                                         || x.Value.Contains(searchTerm)
                                         || x.Description.Contains(searchTerm)
                                         || x.ApiResource.DisplayName.Contains(searchTerm));

            return query;
        }

        public async Task<PageOf<ApiResourceSecretDto>> SelectPage(IQueryable<ApiResourceSecret> query, int pageIndex,
            int pageSize, string sortColumn,
            SortDirection sortDirection, CancellationToken cancellationToken = default)
        {
            return await query.Select(e => new ApiResourceSecretDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret,
                    Type = e.Type,
                    Expiration = e.Expiration,
                    Created = e.Created,
                    ClientId = e.ApiResource.Name,
                    ApiResourceName = e.ApiResource.DisplayName
                })
                .Order(sortColumn ?? "Created", sortDirection)
                .GetPageAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}