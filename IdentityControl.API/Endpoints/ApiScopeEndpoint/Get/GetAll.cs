using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Get
{
    [Authorize(Policy = "AdminOnly")]
    public class GetAll : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiScope> _repository;

        public GetAll(IIdentityRepository<ApiScope> repository)
        {
            _repository = repository;
        }

        [HttpGet("api-scope/all")]
        [SwaggerOperation(Summary = "Gets all the scopes", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<List<ApiScopeDto>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.Query()
                .Select(e => new ApiScopeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    DisplayName = e.DisplayName,
                    Enabled = e.Enabled,
                    Description = e.Description,
                    IsReadOnly = AppConstants.ReadOnlyEntities.AllApiScopes.Contains(e.Name)
                }).ToListAsync(cancellationToken);
        }
    }
}