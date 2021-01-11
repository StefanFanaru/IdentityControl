using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.ApiResourceSecretValidators;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    public class RegenerateBatch : BaseAsyncEndpoint<RegenerateApiResourceSecretRequest[], UpdateApiResourceSecretResponse>
    {
        private readonly IIdentityRepository<ApiResourceSecret> _repository;
        private readonly IAspValidator _validator;

        public RegenerateBatch(IIdentityRepository<ApiResourceSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("api-resource-secret/regenerate-batch")]
        [SwaggerOperation(Summary = "Updates a API Resource Secret", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public override async Task<ActionResult<UpdateApiResourceSecretResponse>> HandleAsync(
            RegenerateApiResourceSecretRequest[] request,
            CancellationToken cancellationToken = default)
        {
            var secretsCount = await _repository.Query()
                .Where(entity => request.Select(r => r.Id)
                    .Contains(entity.Id) && entity.Type != AppConstants.SecretTypes.VisibleOneTime)
                .CountAsync(cancellationToken);

            if (secretsCount == 0 || secretsCount < request.Length)
                return NotFound("One ore more instances could not be updated");

            var toaster = new ToasterEvent(nameof(ApiResourceSecret), ToasterType.Success, ToasterVerbs.Updated, null,
                request.Length);

            foreach (var item in request)
            {
                var validation = await _validator
                    .ValidateAsync<RegenerateApiResourceSecretRequest, UpdateApiResourceSecretResponse,
                            RegenerateApiResourceSecretRequestValidator>
                        (item, toaster, cancellationToken);
                if (validation.Failed) return validation.Response;
            }

            if (_repository.Query().Any(e => request.Any(x => x.Value == e.Value)
                                             && e.Type != AppConstants.SecretTypes.VisibleOneTime))
            {
                return AspExtensions.GetBadRequestWithError<UpdateApiResourceSecretResponse>(
                    "One of the new values is already registered.");
            }

            await _repository.Query()
                .Where(entity => request.Select(r => r.Id)
                    .Contains(entity.Id) && entity.Type != AppConstants.SecretTypes.VisibleOneTime)
                .ForEachAsync(entity => { entity.Value = request.First(r => r.Id == entity.Id).Value.Stamp(); }
                    , cancellationToken);

            await _repository.SaveAsync(toaster, secretsCount);

            return new UpdateApiResourceSecretResponse
            {
                Succeeded = true
            };
        }
    }
}