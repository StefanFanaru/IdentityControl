using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Endpoints.ApiResourceEndpoint.ApiResourceValidators;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Insert
{
    [Authorize(Policy = "AdminOnly")]
    public class Insert : BaseAsyncEndpoint<InsertApiResourceRequest, InsertApiResourceResponse>
    {
        private readonly IIdentityRepository<ApiResource> _apiResourceRepository;
        private readonly IIdentityRepository<ApiResourceScope> _apiResourceScopeRepo;
        private readonly IAspValidator _validator;

        public Insert(IIdentityRepository<ApiResource> apiResourceRepository,
            IIdentityRepository<ApiResourceScope> apiResourceScopeRepo, IAspValidator validator)
        {
            _apiResourceRepository = apiResourceRepository;
            _apiResourceScopeRepo = apiResourceScopeRepo;
            _validator = validator;
        }

        [HttpPost("api-resource")]
        [SwaggerOperation(Summary = "Creates a new API resource", Tags = new[] {"ApiResourceEndpoint"})]
        public override async Task<ActionResult<InsertApiResourceResponse>> HandleAsync(InsertApiResourceRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiResource), ToasterType.Success, ToasterVerbs.Created);
            var validation = await _validator
                .ValidateAsync<InsertApiResourceRequest, InsertApiResourceResponse, InsertApiResourceValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (_apiResourceRepository.Query().Any(e => e.Name == request.Name))
            {
                return AspExtensions.GetBadRequestWithError<InsertApiResourceResponse>(
                    $"ApiResource \"{request.Name}\" already exists.");
            }

            var entity = new ApiResource
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            toaster.Identifier = entity.Name;
            await _apiResourceRepository.InsertAsync(entity);
            await _apiResourceRepository.SaveAsync(toaster);

            // Assignment of API Scopes
            if (request.ApiScopes != null && request.ApiScopes.Length > 0)
            {
                var apiResourceScopes =
                    request.ApiScopes.Select(x => new ApiResourceScope {Scope = x, ApiResourceId = entity.Id});
                await _apiResourceScopeRepo.InsertRange(apiResourceScopes);
            }

            return validation.Response;
        }
    }
}
