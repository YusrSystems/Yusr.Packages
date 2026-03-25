using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Persistence.Context.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users", "public");

            builder.HasKey(x => new { x.TenantId, x.Id });

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(u => new { u.TenantId, u.Username })
                .IsUnique();

            builder.HasOne(x => x.Role)
              .WithMany()
              .HasForeignKey(x => new { x.TenantId, x.RoleId })
              .HasPrincipalKey(x => new { x.TenantId, x.Id })
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Branch)
              .WithMany()
              .HasForeignKey(x => new { x.TenantId, x.BranchId })
              .HasPrincipalKey(x => new { x.TenantId, x.Id })
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TenantId);
        }
    }
}
