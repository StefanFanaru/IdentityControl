using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.ClientEndpoint.ClientChildren
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class SearchClientChildren : BaseAsyncEndpoint
    {
        private readonly IConfigurationRepository<ClientCorsOrigin> _corsRepository;
        private readonly IConfigurationRepository<ClientGrantType> _grantsRepository;
        private readonly IConfigurationRepository<ClientPostLogoutRedirectUri> _logoutRedirectUrisRepository;
        private readonly IConfigurationRepository<ClientRedirectUri> _redirectUriRepository;

        public SearchClientChildren(
            IConfigurationRepository<ClientGrantType> grantsRepository,
            IConfigurationRepository<ClientCorsOrigin> corsRepository,
            IConfigurationRepository<ClientRedirectUri> redirectUriRepository,
            IConfigurationRepository<ClientPostLogoutRedirectUri> logoutRedirectUrisRepository)
        {
            _grantsRepository = grantsRepository;
            _corsRepository = corsRepository;
            _redirectUriRepository = redirectUriRepository;
            _logoutRedirectUrisRepository = logoutRedirectUrisRepository;
        }

        [HttpGet("client/{id}/children/{type}/{searchTerm}")]
        [SwaggerOperation(Summary = "Searches for a Client's child by type", Tags = new[] {"ClientEndpoint"})]
        public async Task<List<ClientChildDto>> HandleAsync(int id, ClientChildType type, string searchTerm,
            CancellationToken cancellationToken = default)
        {
            return type switch
            {
                ClientChildType.GrantType => await _grantsRepository.Query()
                    .Where(e => e.ClientId == id)
                    .Where(e => e.GrantType.Contains(searchTerm))
                    .Select(e => new ClientChildDto {Id = e.Id, Value = e.GrantType})
                    .ToListAsync(cancellationToken),
                ClientChildType.RedirectUri => await _redirectUriRepository.Query()
                    .Where(e => e.ClientId == id)
                    .Where(e => e.RedirectUri.Contains(searchTerm))
                    .Select(e => new ClientChildDto {Id = e.Id, Value = e.RedirectUri})
                    .ToListAsync(cancellationToken),
                ClientChildType.LogoutRedirectUri => await _logoutRedirectUrisRepository.Query()
                    .Where(e => e.ClientId == id)
                    .Where(e => e.PostLogoutRedirectUri.Contains(searchTerm))
                    .Select(e => new ClientChildDto {Id = e.Id, Value = e.PostLogoutRedirectUri})
                    .ToListAsync(cancellationToken),
                ClientChildType.CorsOrigin => await _corsRepository.Query()
                    .Where(e => e.ClientId == id)
                    .Where(e => e.Origin.Contains(searchTerm))
                    .Select(e => new ClientChildDto {Id = e.Id, Value = e.Origin})
                    .ToListAsync(cancellationToken),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}