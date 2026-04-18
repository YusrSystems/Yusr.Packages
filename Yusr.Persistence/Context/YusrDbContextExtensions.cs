using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Utilities;

namespace Yusr.Persistence.Context
{
    public static class DbContextExtensions
    {
        public static void RegisterYusrMath(this ModelBuilder modelBuilder)
        {
            var roundMethod = typeof(YusrMath).GetMethod(nameof(YusrMath.Round), [typeof(decimal)]);
            if (roundMethod == null)
                return;

            modelBuilder
                .HasDbFunction(roundMethod)
                .HasTranslation(args =>
                    new SqlFunctionExpression(
                        "ROUND",
                        [args[0], new SqlFragmentExpression("2")],
                        nullable: true,
                        argumentsPropagateNullability: [true, false],
                        type: typeof(decimal),
                        typeMapping: null
                    )
                );
        }

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
