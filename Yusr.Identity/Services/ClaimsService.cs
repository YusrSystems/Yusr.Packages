using System.Security.Claims;
using Yusr.Core.Abstractions.Constants;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Bus.Identity.Services
{
    public class ClaimsService<SystemPermissions> : IClaimsService where SystemPermissions : ISystemPermissions
    {
        public IEnumerable<Claim> GenerateUserClaims(IUser user, ITenant tenant)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtClaimsConstants.TenantIdClaimName, tenant.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.Name)
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