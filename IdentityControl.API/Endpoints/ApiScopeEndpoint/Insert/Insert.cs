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
using static IdentityControl.API.Endpoints.ApiScopeEndpoint.ApiScopeValidators;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint.Insert
{
    [Authorize(Policy = "AdminOnly")]
    public class Insert : BaseAsyncEndpoint<InsertApiScopeRequest, InsertApiScopeResponse>
    {
        private readonly IIdentityRepository<ApiScope> _repository;
        private readonly IAspValidator _validator;

        public Insert(IIdentityRepository<ApiScope> repository, IAspValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        [HttpPost("api-scope")]
        [SwaggerOperation(Summary = "Creates a new API Scope", Tags = new[] {"ApiScopeEndpoint"})]
        public override async Task<ActionResult<InsertApiScopeResponse>> HandleAsync(InsertApiScopeRequest request,
            CancellationToken cancellationToken = default)
        {
            var toaster = new ToasterEvent(nameof(ApiScope), ToasterType.Success, ToasterVerbs.Created);
            var validation =
                await _validator.ValidateAsync<InsertApiScopeRequest, InsertApiScopeResponse, InsertApiScopeValidator>
                    (request, toaster, cancellationToken);
            if (validation.Failed)
            {
                return validation.Response;
            }

            if (_repository.Query().Any(e => e.Name == request.Name))
            {
                return AspExtensions.GetBadRequestWithError<InsertApiScopeResponse>(
                    $"API Scope \"{request.Name}\" already exists.");
            }

            var entity = new ApiScope
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            toaster.Identifier = entity.Name;
            await _repository.InsertAsync(entity);
            await _repository.SaveAsync(toaster);

            return validation.Response;
        }
    }
}