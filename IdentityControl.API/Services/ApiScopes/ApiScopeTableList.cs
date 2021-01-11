using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ApiScopeEndpoint;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityControl.API.Services.ApiScopes
{
    public class ApiScopeTableList : IApiScopeTableList
    {
        public enum ApiScopeFilter
        {
            Enabled,
            Disabled
        }

        private readonly IIdentityRepository<ApiScope> _repository;

        public ApiScopeTableList(IIdentityRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        public IQueryable<ApiScope> QueryTableList(ApiScopeFilter? filterType, string searchTerm)
        {
            var query = filterType switch
            {
                ApiScopeFilter.Enabled => _repository.Query().Where(x => x.Enabled),
                ApiScopeFilter.Disabled => _repository.Query().Where(x => !x.Enabled),
                _ => _repository.Query()
            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Description.Contains(searchTerm)
                                         || x.Name.Contains(searchTerm)
                                         || x.Description.Contains(searchTerm)
                                         || x.DisplayName.Contains(searchTerm));
            }

            return query;
        }

        public async Task<PageOf<ApiScopeDto>> SelectPage(IQueryable<ApiScope> query, int pageIndex,
            int pageSize, string sortColumn,
            SortDirection sortDirection, CancellationToken cancellationToken = default)
        {
            return await query.Select(e => new ApiScopeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DisplayName = e.DisplayName,
                    Enabled = e.Enabled,
                    Description = e.Description,
                    IsReadOnly = AppConstants.ReadOnlyEntities.AllApiScopes.Contains(e.Name)
                })
                .Order(sortColumn ?? "DisplayName", sortDirection)
                .GetPageAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}