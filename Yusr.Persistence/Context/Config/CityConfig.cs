using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Persistence.Context.Config
{
    public class CityConfig : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.Country)
                 .WithMany()
                 .HasForeignKey(x => x.CountryId)
                 .HasPrincipalKey(x => x.Id)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
