using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Z.EntityFramework.Plus;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Delete
{
    [Authorize(Policy = "AdminOnly")]
    [ApiExplorerSettings(GroupName = "Internal")]
    public class DeleteBatch : BaseAsyncEndpoint
    {
        private readonly IIdentityRepository<ApiResourceSecret> _repository;
        private readonly IUserInfo _userInfo;

        public DeleteBatch(IIdentityRepository<ApiResourceSecret> repository, IUserInfo userInfo)
        {
            _repository = repository;
            _userInfo = userInfo;
        }

        [HttpPatch("api-resource-secret/delete-batch")]
        [SwaggerOperation(Summary = "Deletes multiple secrets", Tags = new[] {"ApiResourceSecretEndpoint"})]
        public async Task<IActionResult> HandleAsync(int[] secretIds, CancellationToken cancellationToken = default)
        {
            var secretsCount = await _repository.Query()
                .Where(x => secretIds.ToList().Contains(x.Id))
                .CountAsync(cancellationToken);

            if (secretsCount == 0 || secretsCount < secretIds.Length)
            {
                return NotFound("One ore more instances where not found");
            }

            await _repository.Query().Where(x => secretIds.Contains(x.Id))
                .DeleteAsync(cancellationToken);
            var toaster = new ToasterEvent(nameof(ApiResourceSecret), ToasterType.Info, ToasterVerbs.Deleted, null, secretsCount);
            await _repository.SaveAsync(toaster, 0);

            return NoContent();
        }
    }
}