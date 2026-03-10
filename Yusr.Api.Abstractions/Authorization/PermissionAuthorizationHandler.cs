using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Yusr.Identity.Abstractions.Constants;

namespace Yusr.Api.Abstractions.Authorization
{
    public class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<PermissionAuthorizationHandler> _logger = logger;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var hasPermission = context.User.Claims
                .Any(c => c.Type == JwtClaimsConstants.PermissionClaimName &&
                          c.Value == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
