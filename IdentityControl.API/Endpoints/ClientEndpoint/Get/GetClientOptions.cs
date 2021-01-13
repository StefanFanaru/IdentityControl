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

namespace IdentityControl.API.Endpoints.ClientEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class GetClientOptions : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;

        public GetClientOptions(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpGet("client/options")]
        [SwaggerOperation(Summary = "Gets possible client options for selection", Tags = new[] {"ClientEndpoint"})]
        public async Task<List<BaseOption<int>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .Select(e => new BaseOption<int>
                {
                    Value = e.Id,
                    Text = e.ClientName // similar to DisplayName on other entities
                }).ToListAsync(cancellationToken);
        }
    }
}