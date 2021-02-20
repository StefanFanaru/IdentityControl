using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientSecretEndpoint.Get;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Z.EntityFramework.Plus;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetPaginated : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResourceSecret> _repository;

        public GetPaginated(IConfigurationRepository<ApiResourceSecret> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-resource-secret/paginated")]
        [SwaggerOperation(Summary = "Gets API Resource secrets paginated", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<GetTableListResponse<ApiResourceSecretDto>> HandleAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var response = new GetTableListResponse<ApiResourceSecretDto>();
            var secrets = await _repository.Query()
                .IncludeOptimized(e => e.ApiResource)
                .Select(e => new ApiResourceSecretDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret,
                    Type = e.Type,
                    Expiration = e.Expiration,
                    Created = e.Created,
                    ClientId = e.ApiResource.Name
                }).ToListAsync(cancellationToken);

            response.Count = secrets.Count;

            if (pageIndex > 0)
            {
                response.Items = secrets.Skip(pageIndex * pageSize).Take(pageSize);
                return response;
            }

            response.Items = secrets.Take(pageSize);
            return response;
        }
    }
}