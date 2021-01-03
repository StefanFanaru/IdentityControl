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
    public class Get : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _repository;

        public Get(IIdentityRepository<ApiResource> repository)
        {
            _repository = repository;
        }


        [HttpGet("api-resource/{id}")]
        [SwaggerOperation(Summary = "Retrieves an API resource", Tags = new[] {"ApiResourceEndpoint"})]
        public async Task<ActionResult<ApiResourceDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _repository.Query()
                .IncludeOptimized(e => e.Secrets)
                .IncludeOptimized(e => e.Scopes)
                .Where(e => e.Id == id).Select(e => new ApiResourceDto
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
                }).FirstOrDefaultAsync(cancellationToken);

            if (response == null) return NotFound($"Instance with ID {id} was not found");

            return response;
        }
    }
}