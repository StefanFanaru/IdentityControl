using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    public class GetTableList : BaseAsyncEndpoint
    {
        public enum ApiResourceFilter
        {
            Enabled,
            Disabled
        }

        private readonly IIdentityRepository<ApiResource> _repository;
        private readonly IAspValidator _validator;

        public GetTableList(IIdentityRepository<ApiResource> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpGet("api-resource/table-list")]
        [SwaggerOperation(Summary = "Gets API Resources in a Table List format", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<PageOf<ApiResourceDto>> HandleAsync(int pageIndex, int pageSize, string sortColumn,
            SortDirection sortDirection, ApiResourceFilter? filterType, string searchTerm,
            CancellationToken cancellationToken = default)
        {
            var query = filterType switch
            {
                ApiResourceFilter.Enabled => _repository.Query().Where(x => x.Enabled),
                ApiResourceFilter.Disabled => _repository.Query().Where(x => !x.Enabled),
                _ => _repository.Query()
            };

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.Description.Contains(searchTerm)
                                         || x.Name.Contains(searchTerm)
                                         || x.Description.Contains(searchTerm)
                                         || x.DisplayName.Contains(searchTerm));

            return await query
                .Select(e => new ApiResourceDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DisplayName = e.DisplayName,
                    Description = e.Description,
                    Enabled = e.Enabled,
                    IsReadOnly = AppConstants.ReadOnlyEntities.AllApiResources.Contains(e.Name),
                    Created = e.Created,
                    Updated = e.Updated
                })
                .Order(sortColumn ?? "DisplayName", sortDirection)
                .GetPageAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}