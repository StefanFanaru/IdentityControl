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
using static IdentityControl.API.Endpoints.ClientSecretEndpoint.ClientSecretValidators;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class RegenerateBatch : BaseAsyncEndpoint<RegenerateClientSecretRequest[], UpdateClientSecretResponse>
    {
        private readonly IIdentityRepository<ClientSecret> _repository;
        private readonly IAspValidator _validator;

        public RegenerateBatch(IIdentityRepository<ClientSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("client-secret/regenerate-batch")]
        [SwaggerOperation(Summary = "Regenerates multiple Client secrets", Tags = new[] {"ClientSecretEndpoint"})]
        public override async Task<ActionResult<UpdateClientSecretResponse>> HandleAsync(RegenerateClientSecretRequest[] request,
            CancellationToken cancellationToken = default)
        {
            var secretsCount = await _repository.Query()
                .Where(entity => request.Select(r => r.Id)
                    .Contains(entity.Id) && entity.Type != AppConstants.SecretTypes.VisibleOneTime)
                .CountAsync(cancellationToken);

            if (secretsCount == 0 || secretsCount < request.Length)
            {
                return NotFound("One ore more instances could not be updated");
            }

            var toaster = new ToasterEvent(nameof(ClientSecret), ToasterType.Success, ToasterVerbs.Updated, null, request.Length);

            foreach (var item in request)
            {
                var validation = await _validator
                    .ValidateAsync<RegenerateClientSecretRequest, UpdateClientSecretResponse, RegenerateRequestValidator>
                        (item, toaster, cancellationToken);
                if (validation.Failed)
                {
                    return validation.Response;
                }
            }

            if (_repository.Query().Any(e => request.Any(x => x.Value == e.Value)
                                             && e.Type != AppConstants.SecretTypes.VisibleOneTime))
            {
                return AspExtensions.GetBadRequestWithError<UpdateClientSecretResponse>(
                    "One of the new values is already registered.");
            }

            await _repository.Query()
                .Where(entity => request.Select(r => r.Id)
                    .Contains(entity.Id) && entity.Type != AppConstants.SecretTypes.VisibleOneTime)
                .ForEachAsync(entity => { entity.Value = request.First(r => r.Id == entity.Id).Value.Stamp(); }
                    , cancellationToken);

            await _repository.SaveAsync(toaster, secretsCount);

            return new UpdateClientSecretResponse
            {
                Succeeded = true
            };
        }
    }
}