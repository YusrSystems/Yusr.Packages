using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yusr.Core.Abstractions.Entities;


namespace Yusr.Persistence.Context.Config
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => new { x.TenantId, x.Id });

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Permissions)
                .HasColumnType("jsonb");

            builder.HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TenantId);
        }
    }
}
