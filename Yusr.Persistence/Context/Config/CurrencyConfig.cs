using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Persistence.Context.Config
{
    public class CurrencyConfig : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
