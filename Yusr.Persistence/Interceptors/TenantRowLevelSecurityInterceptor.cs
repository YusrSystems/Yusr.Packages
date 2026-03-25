using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Yusr.Core.Abstractions.Services;

namespace Yusr.Infrastructure.Persistence.Interceptors
{
    public class TenantRowLevelSecurityInterceptor : DbConnectionInterceptor
    {
        private readonly ITenantService _tenantService;

        public TenantRowLevelSecurityInterceptor(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public override async Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            long? tenantId = _tenantService.CurrentTenantId();

            if (tenantId != null)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT set_config('yusr.tenant_id', @tenantId, false);";

                var p = cmd.CreateParameter();
                p.ParameterName = "tenantId";
                p.Value = tenantId.ToString();
                cmd.Parameters.Add(p);

                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}
