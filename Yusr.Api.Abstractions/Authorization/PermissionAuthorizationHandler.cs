using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Api.Abstractions.Authorization
{
    public class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger, IRolePermissionService rolePermissionService) : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<PermissionAuthorizationHandler> _logger = logger;
        private readonly IRolePermissionService _rolePermissionService = rolePermissionService;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
                return;

            var roleIdClaim = context.User.FindFirst(ClaimTypes.Role);

            if (roleIdClaim == null || !long.TryParse(roleIdClaim.Value, out var roleId))
                return;

            bool hasPermission = await _rolePermissionService.HasPermissionAsync(roleId, requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}