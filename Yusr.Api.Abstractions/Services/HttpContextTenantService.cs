using Microsoft.AspNetCore.Http;
using Yusr.Core.Abstractions.Services;
using Yusr.Identity.Abstractions.Constants;

namespace Yusr.Api.Abstractions.Services
{
    public class HttpContextTenantService(IHttpContextAccessor httpContextAccessor) : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private long? _tenantId;

        public long? CurrentTenantId()
        {
            if (long.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst(JwtClaimsConstants.TenantIdClaimName)?.Value, out long res))
                return res;

            return _tenantId;
        }

        public void SetTenant(long tenantId)
        {
            _tenantId = tenantId;
        }
    }
}
