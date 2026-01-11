using BloodBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence.Configurations
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("Donations");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.UnitsDonated)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(d => d.Donor)
                .WithMany(dn => dn.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.BloodType)
                .WithMany(bt => bt.Donations)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
