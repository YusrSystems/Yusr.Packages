using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Services;
using Yusr.Persistence.Converters;

namespace Yusr.Persistence.Context
{
    public partial class YusrDbContext(DbContextOptions<YusrDbContext> options, ITenantService tenantService) : DbContext(options)
    {
        private readonly ITenantService _tenantService = tenantService;

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeUtcConverter>();
            configurationBuilder.Properties<DateTime?>().HaveConversion<DateTimeUtcConverter>();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ApplyTenantRules(_tenantService.CurrentTenantId());
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.ApplyTenantRules(_tenantService.CurrentTenantId());
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            this.ApplyTenantRules(_tenantService.CurrentTenantId());
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyTenantRules(_tenantService.CurrentTenantId());
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected void SetTenantQueryFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseTenantEntity
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => x.TenantId == _tenantService.CurrentTenantId());
        }
    }
}