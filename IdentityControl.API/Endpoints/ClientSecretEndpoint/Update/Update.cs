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
using static IdentityControl.API.Endpoints.ClientSecretEndpoint.ClientSecretValidators;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "IdentityServer")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ClientSecret> _repository;
        private readonly IAspValidator _validator;

        public Update(IIdentityRepository<ClientSecret> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPatch("client-secret/{id}")]
        [SwaggerOperation(Summary = "Updates a Client secret", Tags = new[] {"ClientSecretEndpoint"})]
        public async Task<ActionResult<UpdateClientSecretResponse>> HandleAsync(int id, UpdateClientSecretRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ClientSecret), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator.ValidateAsync<UpdateClientSecretRequest, UpdateClientSecretResponse, UpdateRequestValidator>
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
            entity.ClientId = request.OwnerId;
            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}