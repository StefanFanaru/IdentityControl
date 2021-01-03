using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Z.EntityFramework.Plus;
using static IdentityControl.API.Endpoints.ApiResourceEndpoint.ApiResourceValidators;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResource> _apiResourceRepository;
        private readonly IIdentityRepository<ApiResourceScope> _apiResourceScopeRepo;
        private readonly IAspValidator _validator;

        public Update(IIdentityRepository<ApiResource> apiResourceRepository,
            IIdentityRepository<ApiResourceScope> apiResourceScopeRepo, IAspValidator validator)
        {
            _apiResourceRepository = apiResourceRepository;
            _apiResourceScopeRepo = apiResourceScopeRepo;
            _validator = validator;
        }

        [HttpPatch("api-resource/{id}")]
        [SwaggerOperation(Summary = "Updates an API resource", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<ActionResult<UpdateApiResourceResponse>> HandleAsync(int id, UpdateApiResourceRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator.ValidateAsync<UpdateApiResourceRequest, UpdateApiResourceResponse, UpdateApiResourceValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (!_apiResourceRepository.Query()
                .Any(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope))
            {
                return NotFound(id);
            }

            if (_apiResourceRepository.Query().Any(e => e.Name == request.Name && e.Id != id))
            {
                return AspExtensions.GetBadRequestWithError<UpdateApiResourceResponse>(
                    $"API Resource \"{request.Name}\" already exists.");
            }

            await _apiResourceRepository.Query().Where(e => e.Id == id)
                .UpdateAsync(x => new ApiResource
                {
                    Name = request.Name,
                    Description = request.Description,
                    DisplayName = request.DisplayName
                }, cancellationToken);

            // Assignment of API Scopes
            if (request.ApiScopes != null && request.ApiScopes.Length > 0)
            {
                // Clean current relations
                await _apiResourceScopeRepo.Query().Where(x => x.ApiResourceId == id).DeleteAsync(cancellationToken);

                // Add new relations
                var apiResourceScopes =
                    request.ApiScopes.Select(x => new ApiResourceScope {Scope = x, ApiResourceId = id});
                await _apiResourceScopeRepo.InsertRange(apiResourceScopes);
                await _apiResourceRepository.SaveAsync(toaster);
            }

            return validation.Response;
        }
    }
}