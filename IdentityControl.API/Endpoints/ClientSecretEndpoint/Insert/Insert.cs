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
using static IdentityControl.API.Endpoints.ClientSecretEndpoint.ClientSecretValidators;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Insert
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Insert : BaseAsyncEndpoint<InsertClientSecretRequest, InsertClientSecretResponse>
    {
        private readonly IConfigurationRepository<ClientSecret> _repository;
        private readonly IAspValidator _validator;

        public Insert(IConfigurationRepository<ClientSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPost("client-secret")]
        [SwaggerOperation(Summary = "Creates a new Client secret", Tags = new[] {"ClientSecretEndpoint"})]
        public override async Task<ActionResult<InsertClientSecretResponse>> HandleAsync(InsertClientSecretRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ClientSecret), ToasterType.Success, ToasterVerbs.Created);
            var validation =
                await _validator.ValidateAsync<InsertClientSecretRequest, InsertClientSecretResponse, InsertRequestValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (_repository.Query().Any(e => e.Value == request.Value && e.Type != AppConstants.SecretTypes.VisibleOneTime))
            {
                return AspExtensions.GetBadRequestWithError<InsertClientSecretResponse>("This client secret already exists.");
            }

            var entity = new ClientSecret
            {
                Created = DateTime.UtcNow,
                Description = request.Description,
                Type = request.Type,
                Expiration = request.Expiration,
                Value = request.Value.Stamp(),
                ClientId = request.OwnerId
            };

            await _repository.InsertAsync(entity);
            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}