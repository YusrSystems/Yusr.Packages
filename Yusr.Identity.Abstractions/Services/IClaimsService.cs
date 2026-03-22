using System.Security.Claims;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Identity.Abstractions.Services
{
    public interface IClaimsService
    {
        IEnumerable<Claim> GenerateUserClaims(User user, Tenant tenant);
    }
}
