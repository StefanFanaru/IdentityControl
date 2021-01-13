using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class GetAll : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _repository;

        public GetAll(IIdentityRepository<Client> repository)
        {
            _repository = repository;
        }

        [HttpGet("client/all")]
        [SwaggerOperation(Summary = "Gets all the scopes", Tags = new[] {"ClientEndpoint"})]
        public async Task<List<ClientDto>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query().SelectBasicClientDto().ToListAsync(cancellationToken);
        }
    }
}