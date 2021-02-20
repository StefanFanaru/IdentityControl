using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Get : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ApiScope> _repository;

        public Get(IConfigurationRepository<ApiScope> repository)
        {
            _repository = repository;
        }


        [HttpGet("api-scope/{id}")]
        [SwaggerOperation(Summary = "Retrieves an API Scope", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<ActionResult<ApiScopeDto>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _repository.Query().Where(e => e.Id == id).Select(e => new ApiScopeDto
            {
                Id = e.Id,
                Name = e.Name,
                DisplayName = e.DisplayName,
                Description = e.Description,
                Enabled = e.Enabled,
                IsReadOnly = AppConstants.ReadOnlyEntities.AllApiScopes.Contains(e.Name)
            }).FirstOrDefaultAsync(cancellationToken);

            if (response == null)
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            return response;
        }
    }
}