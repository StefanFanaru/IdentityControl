using System;
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
using Swashbuckle.AspNetCore.Annotations;
using static IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.ApiResourceSecretValidators;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Insert
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class Insert : BaseAsyncEndpoint<InsertApiResourceSecretRequest, InsertApiResourceSecretResponse>
    {
        private readonly IIdentityRepository<ApiResourceSecret> _repository;
        private readonly IAspValidator _validator;

        public Insert(IIdentityRepository<ApiResourceSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPost("api-resource-secret")]
        [SwaggerOperation(Summary = "Creates a new API Resource Secret", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public override async Task<ActionResult<InsertApiResourceSecretResponse>> HandleAsync(
            InsertApiResourceSecretRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiResourceSecret), ToasterType.Success, ToasterVerbs.Created);
            var validation =
                await _validator
                    .ValidateAsync<InsertApiResourceSecretRequest, InsertApiResourceSecretResponse,
                            InsertApiResourceSecretRequestValidator>
                        (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (_repository.Query().Any(e => e.Value == request.Value && e.Type != AppConstants.SecretTypes.VisibleOneTime))
            {
                return AspExtensions.GetBadRequestWithError<InsertApiResourceSecretResponse>("This secret already exists.");
            }

            var entity = new ApiResourceSecret
            {
                Created = DateTime.UtcNow,
                Description = request.Description,
                Type = request.Type,
                Expiration = request.Expiration,
                Value = request.Value.Stamp(),
                ApiResourceId = request.OwnerId
            };

            await _repository.InsertAsync(entity);
            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}