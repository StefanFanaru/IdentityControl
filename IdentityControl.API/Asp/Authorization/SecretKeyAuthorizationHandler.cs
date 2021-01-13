using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace IdentityControl.API.Asp.Authorization
{
    public class SecretKeyAuthorizationHandler : AuthorizationHandler<SecretKeyRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecretKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SecretKeyRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var header = httpContext.Request.Headers
                    .FirstOrDefault(h => string.Equals(h.Key, "SecretKey", System.StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (!string.IsNullOrWhiteSpace(header) && requirement.ContainsSecret(header))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}