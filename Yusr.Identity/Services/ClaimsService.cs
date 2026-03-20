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
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtClaimsConstants.TenantIdClaimName, tenant.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };
        }
    }
}