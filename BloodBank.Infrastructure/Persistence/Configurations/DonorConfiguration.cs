using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence.Configurations
{
    public class DonorConfiguration : IEntityTypeConfiguration<Donor>
    {
        public void Configure(EntityTypeBuilder<Donor> builder)
        {
            builder.ToTable("Donors");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.HealthStatus)
                .HasMaxLength(200);

            builder.HasOne(d => d.BloodType)
                .WithMany(bt => bt.Donors)
                .HasForeignKey(d => d.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
               .WithOne()
               .HasForeignKey<Donor>(d => d.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
