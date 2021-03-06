﻿using System;
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
    public class ClientChildDelete : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<Client> _clientRepository;
        private readonly IConfigurationRepository<ClientCorsOrigin> _corsRepository;
        private readonly IConfigurationRepository<ClientGrantType> _grantsRepository;
        private readonly IConfigurationRepository<ClientPostLogoutRedirectUri> _logoutRedirectUrisRepository;
        private readonly IConfigurationRepository<ClientRedirectUri> _redirectUriRepository;

        public ClientChildDelete(
            IConfigurationRepository<Client> clientRepository,
            IConfigurationRepository<ClientGrantType> grantsRepository,
            IConfigurationRepository<ClientCorsOrigin> corsRepository,
            IConfigurationRepository<ClientRedirectUri> redirectUriRepository,
            IConfigurationRepository<ClientPostLogoutRedirectUri> logoutRedirectUrisRepository)
        {
            _clientRepository = clientRepository;
            _grantsRepository = grantsRepository;
            _corsRepository = corsRepository;
            _redirectUriRepository = redirectUriRepository;
            _logoutRedirectUrisRepository = logoutRedirectUrisRepository;
        }

        [HttpDelete("client/{id}/children/{childType}/delete/{childId}")]
        [SwaggerOperation(Summary = "Delete a grant type, CORS Origin, or redirect URIs of a Client",
            Tags = new[] {"ClientEndpoint"})]
        public async Task<ActionResult<ClientChildAssignmentResponse>> HandleAsync(int id, ClientChildType childType, int childId,
            CancellationToken cancellationToken = default)
        {
            if (!_clientRepository.Query()
                .Any(e => e.Id == id && e.ClientId != AppConstants.ReadOnlyEntities.AngularClient))
            {
                return NotFound(id);
            }

            var toaster = new ToasterEvent(nameof(Client), ToasterType.Info, ToasterVerbs.Deleted);

            switch (childType)
            {
                case ClientChildType.GrantType:
                    _grantsRepository.Delete(childId);
                    await _grantsRepository.SaveAsync(toaster);
                    break;
                case ClientChildType.RedirectUri:
                    _redirectUriRepository.Delete(childId);
                    await _redirectUriRepository.SaveAsync(toaster);
                    break;
                case ClientChildType.LogoutRedirectUri:
                    _logoutRedirectUrisRepository.Delete(childId);
                    await _logoutRedirectUrisRepository.SaveAsync(toaster);
                    break;
                case ClientChildType.CorsOrigin:
                    _corsRepository.Delete(childId);
                    await _corsRepository.SaveAsync(toaster);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return NoContent();
        }
    }
}