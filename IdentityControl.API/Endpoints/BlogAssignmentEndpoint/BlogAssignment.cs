using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Data;
using IdentityControl.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityControl.API.Endpoints.BlogAssignmentEndpoint
{
    [Authorize(Policy = "BlogSecretKey")]
    [ApiExplorerSettings(GroupName = "Integration")]
    public class BlogAssignment : BaseAsyncEndpoint
    {
        private readonly IUserRepository _userRepository;

        public BlogAssignment(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("user/{id}/assign-blog/{blogId}")]
        [SwaggerOperation(Summary = "Assign a blog to a User",
            Tags = new[] {"BloggingAPI"})]
        public async Task<IActionResult> HandleAsync(string id, string blogId, CancellationToken cancellationToken = default)
        {
            if (!await _userRepository.Query().Where(x => x.Id == id).AnyAsync(cancellationToken))
            {
                return NotFound($"Instance with ID {id} was not found");
            }

            var hasBlog = await _userRepository.Query().Where(x => x.Id == id && x.BlogId != null).AnyAsync(cancellationToken);
            var blogAlreadyAssigned = await _userRepository.Query().Where(x => x.BlogId == blogId).AnyAsync(cancellationToken);

            if (hasBlog)
            {
                return Forbid($"User with ID {id} already has a blog assigned");
            }

            if (blogAlreadyAssigned)
            {
                return BadRequest($"Blog with ID {blogId} is already assigned to another user");
            }

            await _userRepository.Query().Where(x => x.Id == id)
                .UpdateFromQueryAsync(x => new ApplicationUser {BlogId = blogId}, cancellationToken);

            return Ok();
        }
    }
}