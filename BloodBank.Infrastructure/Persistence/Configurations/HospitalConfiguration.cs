using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence.Configurations
{
    public class HospitalConfiguration : IEntityTypeConfiguration<Hospital>
    {
        public void Configure(EntityTypeBuilder<Hospital> builder)
        {
            builder.ToTable("Hospitals");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Address)
                .HasMaxLength(300);

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Hospital>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
