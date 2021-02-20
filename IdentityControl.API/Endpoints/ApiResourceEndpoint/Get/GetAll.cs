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
using Z.EntityFramework.Plus;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class GetAll : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiResource> _repository;

        public GetAll(IConfigurationRepository<ApiResource> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-resource/all")]
        [SwaggerOperation(Summary = "Gets all the API resources", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<List<ApiResourceDto>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .IncludeOptimized(e => e.Secrets)
                .IncludeOptimized(e => e.Scopes)
                .Select(e => new ApiResourceDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DisplayName = e.DisplayName,
                    Description = e.Description,
                    Enabled = e.Enabled,
                    IsReadOnly = e.NonEditable,
                    Created = e.Created,
                    Scopes = e.Scopes.Select(x => new BaseOption<int>
                    {
                        Value = x.Id,
                        Text = x.Scope
                    }).ToList(),
                    Secrets = e.Secrets.Select(x => new BaseOption<int>
                    {
                        Value = x.Id,
                        Text = x.Description
                    }).ToList()
                }).ToListAsync(cancellationToken);
        }
    }
}