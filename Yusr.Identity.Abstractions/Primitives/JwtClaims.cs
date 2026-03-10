using System.Security.Claims;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Identity.Abstractions.Constants;

namespace Yusr.Identity.Abstractions.Primitives
{
    public class JwtClaims
    {
        public long UserId { get; set; }
        public long TenantId { get; set; }

        public static OperationResult<JwtClaims> ExtractClaims(ClaimsPrincipal user)
        {
            if (user == null)
                return OperationResult<JwtClaims>.BadRequest("User principal is null");

            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantIdStr = user.FindFirst(JwtClaimsConstants.TenantIdClaimName)?.Value;

            // 1. Validate Nulls
            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out long userId))
                return OperationResult<JwtClaims>.BadRequest("User ID is missing or invalid.");

            if (string.IsNullOrEmpty(tenantIdStr) || !long.TryParse(tenantIdStr, out long tenantId))
                return OperationResult<JwtClaims>.BadRequest("TenantId is missing or invalid.");

            var claims = new JwtClaims
            {
                UserId = userId,
                TenantId = tenantId
            };

            return OperationResult<JwtClaims>.Ok(claims);
        }
    }
}
