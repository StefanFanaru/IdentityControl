using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientSecretEndpoint.Get;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetPaginated : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiScope> _repository;

        public GetPaginated(IConfigurationRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-scope/paginated")]
        [SwaggerOperation(Summary = "Gets API scopes paginated", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<GetTableListResponse<ApiScopeDto>> HandleAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var response = new GetTableListResponse<ApiScopeDto>();
            var apiScopes = await _repository.Query()
                .Select(e => new ApiScopeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DisplayName = e.DisplayName,
                    Enabled = e.Enabled,
                    Description = e.Description,
                    IsReadOnly = AppConstants.ReadOnlyEntities.AllApiScopes.Contains(e.Name)
                }).ToListAsync(cancellationToken);

            response.Count = apiScopes.Count;

            if (pageIndex > 0)
            {
                response.Items = apiScopes.Skip(pageIndex * pageSize).Take(pageSize);
                return response;
            }

            response.Items = apiScopes.Take(pageSize);
            return response;
        }
    }
}