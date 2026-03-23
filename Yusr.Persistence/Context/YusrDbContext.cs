using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Services;
using Yusr.Persistence.Converters;

namespace Yusr.Persistence.Context
{
    public class YusrDbContext(DbContextOptions<YusrDbContext> options, ITenantService tenantService) : DbContext(options)
    {
        private readonly ITenantService _tenantService = tenantService;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(YusrDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseTenantEntity).IsAssignableFrom(entityType.ClrType) && !entityType.ClrType.IsAbstract)
                {
                    var method = typeof(YusrDbContext)
                        .GetMethod(nameof(SetTenantQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);

                    method?.Invoke(this, new object[] { modelBuilder });
                }
            }
        }
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


        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}