using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Common.Extensions;
using IdentityControl.API.Data;
using IdentityControl.API.Services.SignalR;
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
    public class Regenerate : BaseAsyncEndpoint<RegenerateApiResourceSecretRequest, UpdateApiResourceSecretResponse>
    {
        private readonly IIdentityRepository<ApiResourceSecret> _repository;
        private readonly IAspValidator _validator;

        public Regenerate(IIdentityRepository<ApiResourceSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("api-resource-secret/regenerate")]
        [SwaggerOperation(Summary = "Updates a API Resource Secret", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public override async Task<ActionResult<UpdateApiResourceSecretResponse>> HandleAsync(
            RegenerateApiResourceSecretRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiResourceSecret), ToasterType.Success, ToasterVerbs.Updated);
            var validation = await _validator
                .ValidateAsync<RegenerateApiResourceSecretRequest, UpdateApiResourceSecretResponse,
                        RegenerateApiResourceSecretRequestValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (!_repository.Query().Any(e =>
                e.Id == request.Id && e.Type != AppConstants.SecretTypes.VisibleOneTime))
                return NotFound(request.Id);


            if (_repository.Query().Any(e => e.Value == request.Value && e.Type != AppConstants.SecretTypes.VisibleOneTime))
            {
                return AspExtensions.GetBadRequestWithError<UpdateApiResourceSecretResponse>(
                    "This client secret already exists.");
            }

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            entity.Value = request.Value.Stamp();

            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}
