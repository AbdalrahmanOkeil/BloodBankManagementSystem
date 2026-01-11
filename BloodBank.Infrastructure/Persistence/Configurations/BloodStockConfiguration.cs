using BloodBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence.Configurations
{
    public class BloodStockConfiguration : IEntityTypeConfiguration<BloodStock>
    {
        public void Configure(EntityTypeBuilder<BloodStock> builder)
        {
            builder.ToTable("BloodStocks");

            builder.HasKey(bs => bs.Id);

            builder.Property(bs => bs.UnitsAvailable)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
