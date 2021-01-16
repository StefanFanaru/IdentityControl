using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Z.EntityFramework.Plus;
using static IdentityControl.API.Endpoints.ClientEndpoint.ClientValidators;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Update
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Update : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<Client> _clientRepository;
        private readonly IIdentityRepository<ClientScope> _clientScopeRepository;
        private readonly IAspValidator _validator;

        public Update(IIdentityRepository<Client> clientRepository, IIdentityRepository<ClientScope> clientScopeRepository,
            IAspValidator validator)
        {
            _clientRepository = clientRepository;
            _clientScopeRepository = clientScopeRepository;
            _validator = validator;
        }

        [HttpPatch("client/{id}")]
        [SwaggerOperation(Summary = "Updates a Client", Tags = new[] {"ClientEndpoint"})]
        public async Task<ActionResult<UpdateClientResponse>> HandleAsync(int id, UpdateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Updated);
            var validation =
                await _validator.ValidateAsync<UpdateClientRequest, UpdateClientResponse, UpdateClientValidator>
                    (request, toaster, cancellationToken);

            if (validation.Failed)
            {
                return validation.Response;
            }

            // if (!_clientRepository.Query().Any(e => e.Id == id && e.ClientId != AppConstants.ReadOnlyEntities.AngularClient))
            if (!_clientRepository.Query().Any(e => e.Id == id))
            {
                return NotFound(id);
            }

            if (_clientRepository.Query()
                .Any(e => (e.ClientId == request.Name || e.ClientUri == request.ClientUri) && e.Id != id))
            {
                return AspExtensions.GetBadRequestWithError<UpdateClientResponse>($"Client \"{request.Name}\" already exists.");
            }

            await _clientRepository.Query().Where(e => e.Id == id).UpdateAsync(x => new Client
            {
                ClientId = request.Name,
                ClientName = request.DisplayName,
                Description = request.Description,
                NonEditable = request.IsReadOnly,
                Updated = DateTime.UtcNow,
                RequirePkce = request.RequirePkce,
                AccessTokenLifetime = request.AccessTokenLifetime * 60, // transform minutes in seconds,
                ClientUri = request.ClientUri,
                AllowOfflineAccess = request.AllowOfflineAccess,
                RequireClientSecret = request.RequireClientSecret,
                AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser
            }, cancellationToken);

            // Assignment of API Scopes
            if (request.ApiScopes != null && request.ApiScopes.Length > 0)
            {
                // Clean current relations
                await _clientScopeRepository.Query().Where(x => x.ClientId == id).DeleteAsync(cancellationToken);

                // Add new ones
                var clientApiScopes = request.ApiScopes.Select(x => new ClientScope {Scope = x, ClientId = id});
                await _clientScopeRepository.InsertRange(clientApiScopes);
                await _clientRepository.SaveAsync(toaster);
            }

            return validation.Response;
        }
    }
}