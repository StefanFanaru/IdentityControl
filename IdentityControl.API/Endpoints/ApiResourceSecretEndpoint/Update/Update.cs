using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
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
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResourceSecret> _repository;
        private readonly IAspValidator _validator;

        public Update(IIdentityRepository<ApiResourceSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("api-resource-secret/{id}")]
        [SwaggerOperation(Summary = "Updates a API Resource Secret", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<ActionResult<UpdateApiResourceSecretResponse>> HandleAsync(int id,
            UpdateApiResourceSecretRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiResourceSecret), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator
                    .ValidateAsync<UpdateApiResourceSecretRequest, UpdateApiResourceSecretResponse,
                            UpdateApiResourceSecretRequestValidator>
                        (request, toaster, cancellationToken);
            if (validation.Failed)
            {
                return validation.Response;
            }

            if (!_repository.Query().Any(e => e.Id == id && e.Type != AppConstants.SecretTypes.VisibleOneTime))
            {
                return NotFound(id);
            }

            var entity = await _repository.Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            entity.Expiration = request.ExpiresAt;
            entity.Description = request.Description;
            entity.ApiResourceId = request.OwnerId;

            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}