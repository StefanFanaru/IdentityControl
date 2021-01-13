using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class GetResourceOptions : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _repository;

        public GetResourceOptions(IIdentityRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-resource/options")]
        [SwaggerOperation(Summary = "Gets possible API Resource options for selection", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<List<BaseOption<int>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .Select(e => new BaseOption<int>
                {
                    Value = e.Id,
                    Text = e.DisplayName
                }).ToListAsync(cancellationToken);
        }
    }
}