using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientEndpoint.ClientChildren
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class ClientChildAssignment : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _clientRepository;
        private readonly IIdentityRepository<ClientCorsOrigin> _corsRepository;
        private readonly IIdentityRepository<ClientGrantType> _grantsRepository;
        private readonly IIdentityRepository<ClientPostLogoutRedirectUri> _logoutRedirectUrisRepository;
        private readonly IIdentityRepository<ClientRedirectUri> _redirectUriRepository;
        private readonly IAspValidator _validator;

        public ClientChildAssignment(
            IAspValidator validator,
            IIdentityRepository<Client> clientRepository,
            IIdentityRepository<ClientGrantType> grantsRepository,
            IIdentityRepository<ClientCorsOrigin> corsRepository,
            IIdentityRepository<ClientRedirectUri> redirectUriRepository,
            IIdentityRepository<ClientPostLogoutRedirectUri> logoutRedirectUrisRepository)
        {
            _validator = validator;
            _clientRepository = clientRepository;
            _grantsRepository = grantsRepository;
            _corsRepository = corsRepository;
            _redirectUriRepository = redirectUriRepository;
            _logoutRedirectUrisRepository = logoutRedirectUrisRepository;
        }

        [HttpPost("client/{id}/children/assignment")]
        [SwaggerOperation(Summary = "Assign a grant type, CORS Origin, or redirect URIs to a Client",
            Tags = new[] {"ClientEndpoint"})]
        public async Task<ActionResult<ClientChildAssignmentResponse>> HandleAsync(int id, ClientChildAssignmentRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator
                    .ValidateAsync<ClientChildAssignmentRequest, ClientChildAssignmentResponse,
                            ClientValidators.ClientChildAssignmentValidator>
                        (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            if (!_clientRepository.Query()
                .Any(e => e.Id == id && e.ClientId != AppConstants.ReadOnlyEntities.AngularClient))
            {
                return NotFound(id);
            }

            switch (request.Type)
            {
                case ClientChildType.GrantType:
                    await _grantsRepository.InsertAsync(new ClientGrantType
                    {
                        ClientId = id,
                        GrantType = request.Value
                    });
                    await _grantsRepository.SaveAsync(toaster);
                    break;
                case ClientChildType.RedirectUri:
                    await _redirectUriRepository.InsertAsync(new ClientRedirectUri
                    {
                        ClientId = id,
                        RedirectUri = request.Value
                    });
                    await _redirectUriRepository.SaveAsync(toaster);
                    break;
                case ClientChildType.LogoutRedirectUri:
                    await _logoutRedirectUrisRepository.InsertAsync(new ClientPostLogoutRedirectUri
                    {
                        ClientId = id,
                        PostLogoutRedirectUri = request.Value
                    });
                    await _logoutRedirectUrisRepository.SaveAsync(toaster);
                    break;
                case ClientChildType.CorsOrigin:
                    await _corsRepository.InsertAsync(new ClientCorsOrigin
                    {
                        ClientId = id,
                        Origin = request.Value
                    });
                    await _corsRepository.SaveAsync(toaster);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return validation.Response;
        }
    }
}