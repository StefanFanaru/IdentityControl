using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Z.EntityFramework.Plus;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetPaginated : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ClientSecret> _repository;

        public GetPaginated(IIdentityRepository<ClientSecret> repository)
        {
            _repository = repository;
        }

        [HttpGet("client-secret/paginated")]
        [SwaggerOperation(Summary = "Gets Client Secrets paginated", Tags = new[] {"ClientSecretEndpoint"})]
        public async Task<GetTableListResponse<ClientSecretDto>> HandleAsync(int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var response = new GetTableListResponse<ClientSecretDto>();
            var secrets = await _repository.Query()
                .IncludeOptimized(e => e.Client)
                .Select(e => new ClientSecretDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Value = e.Value.Unstamp(), // Very important to Unstamp a secret
                    Type = e.Type,
                    Expiration = e.Expiration,
                    Created = e.Created,
                    ClientName = e.Client.ClientName
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