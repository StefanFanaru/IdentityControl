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
using static IdentityControl.API.Endpoints.ClientEndpoint.ClientValidators;

namespace IdentityControl.API.Endpoints.ClientEndpoint.Insert
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class Insert : BaseAsyncEndpoint<InsertClientRequest, InsertClientResponse>
    {
        private readonly IIdentityRepository<Client> _clientRepository;
        private readonly IIdentityRepository<ClientScope> _clientScopeRepository;
        private readonly IAspValidator _validator;

        public Insert(IIdentityRepository<Client> clientRepository, IIdentityRepository<ClientScope> clientScopeRepository,
            IAspValidator validator)
        {
            _clientRepository = clientRepository;
            _clientScopeRepository = clientScopeRepository;
            _validator = validator;
        }

        [HttpPost("client")]
        [SwaggerOperation(Summary = "Creates a new Client", Tags = new[] {"ClientEndpoint"})]
        public override async Task<ActionResult<InsertClientResponse>> HandleAsync(InsertClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(Client), ToasterType.Success, ToasterVerbs.Created);
            var validation =
                await _validator.ValidateAsync<InsertClientRequest, InsertClientResponse, InsertClientValidator>
                    (request, toaster, cancellationToken);
            if (validation.Failed)
            {
                return validation.Response;
            }

            if (_clientRepository.Query().Any(e => e.ClientId == request.Name || e.ClientUri == request.ClientUri))
            {
                return AspExtensions.GetBadRequestWithError<InsertClientResponse>($"Client \"{request.Name}\" already exists.");
            }

            var entity = new Client
            {
                ClientId = request.Name,
                ClientName = request.DisplayName,
                Description = request.Description,
                NonEditable = request.IsReadOnly,
                Created = DateTime.UtcNow,
                RequirePkce = request.RequirePkce,
                AccessTokenLifetime = request.AccessTokenLifetime * 60, // transform minutes in seconds
                ClientUri = request.ClientUri,
                AllowOfflineAccess = request.AllowOfflineAccess,
                RequireClientSecret = request.RequireClientSecret,
                AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser
            };

            toaster.Identifier = entity.ClientName;
            await _clientRepository.InsertAsync(entity);
            await _clientRepository.SaveAsync(toaster);

            // Assignment of API Scopes
            if (request.ApiScopes != null && request.ApiScopes.Length > 0)
            {
                var clientApiScopes = request.ApiScopes.Select(x => new ClientScope {Scope = x, ClientId = entity.Id});
                await _clientScopeRepository.InsertRange(clientApiScopes);
            }

            return validation.Response;
        }
    }
}