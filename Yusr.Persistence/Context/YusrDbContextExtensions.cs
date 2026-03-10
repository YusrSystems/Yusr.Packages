using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Persistence.Context
{
    public static class DbContextExtensions
    {
        public static void ApplyTenantRules(this DbContext context, long? tenantId)
        {
            if (tenantId == null) return;

            foreach (var entry in context.ChangeTracker.Entries<BaseTenantEntity>())
            {
                // INSERT
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = tenantId.Value;
                }

                // UPDATE
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.TenantId).IsModified = false;
                }
            }
        }
    }
}
