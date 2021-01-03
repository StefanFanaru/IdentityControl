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
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Z.EntityFramework.Plus;
using static IdentityControl.API.Endpoints.ApiScopeEndpoint.ApiScopeValidators;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResourceScope> _apiResourceScopeRepo;
        private readonly IIdentityRepository<ApiScope> _apiScopeRepository;
        private readonly IIdentityRepository<ClientScope> _clientScopeRepository;
        private readonly IAspValidator _validator;

        public Update(
            IIdentityRepository<ApiScope> apiScopeRepository,
            IIdentityRepository<ClientScope> clientScopeRepository,
            IIdentityRepository<ApiResourceScope> apiResourceScopeRepo,
            IAspValidator validator)
        {
            _apiScopeRepository = apiScopeRepository;
            _clientScopeRepository = clientScopeRepository;
            _apiResourceScopeRepo = apiResourceScopeRepo;
            _validator = validator;
        }

        [HttpPatch("api-scope/{id}")]
        [SwaggerOperation(Summary = "Updates an API scope", Tags = new[] {"ApiScopeEndpoint"})]
        public async Task<ActionResult<UpdateApiScopeResponse>> HandleAsync(int id, UpdateApiScopeRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator.ValidateAsync<UpdateApiScopeRequest, UpdateApiScopeResponse, UpdateApiScopeValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (!_apiScopeRepository.Query()
                .Any(e => e.Id == id && e.Name != AppConstants.ReadOnlyEntities.IdentityControlApiScope))
            {
                return NotFound(id);
            }

            if (_apiScopeRepository.Query().Any(e => e.Name == request.Name && e.Id != id))
            {
                return AspExtensions.GetBadRequestWithError<UpdateApiScopeResponse>(
                    $"API Scope \"{request.Name}\" already exists.");
            }

            var entity = await _apiScopeRepository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity.Name != request.Name)
            {
                await _clientScopeRepository.Query()
                    .Where(x => x.Scope == entity.Name)
                    .UpdateAsync(x => new ClientScope {Scope = request.Name}, cancellationToken);

                await _apiResourceScopeRepo.Query()
                    .Where(x => x.Scope == entity.Name)
                    .UpdateAsync(x => new ApiResourceScope {Scope = request.Name}, cancellationToken);
                entity.Name = request.Name;
            }

            entity.Description = request.Description;
            entity.DisplayName = request.DisplayName;

            toaster.Identifier = entity.Name;
            await _apiScopeRepository.SaveAsync(toaster);
            return validation.Response;
        }
    }
}
