using System.Security.Claims;
using Yusr.Core.Abstractions.Entities;
using Yusr.Identity.Abstractions.Constants;
using Yusr.Identity.Abstractions.Interfaces;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Identity.Services
{
    public class ClaimsService<SystemPermissions> : IClaimsService where SystemPermissions : ISystemPermissions
    {
        public IEnumerable<Claim> GenerateUserClaims(IUser user, ITenant tenant)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(JwtClaimsConstants.TenantIdClaimName, tenant.Id.ToString()),
                new(ClaimTypes.Role, user.Role.Name)
            };

            var activePermissions = user.Role.Permissions
                .Where(p => SystemPermissions.All.Contains(p))
                .ToList();

            foreach (var permission in activePermissions)
            {
                claims.Add(new Claim(JwtClaimsConstants.PermissionClaimName, permission));
            }

            return claims;
        }
    }
}