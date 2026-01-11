using BloodBank.Domain.Entities;
using BloodBank.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence.Configurations
{
    public class BloodRequestConfiguration : IEntityTypeConfiguration<BloodRequest>
    {
        public void Configure(EntityTypeBuilder<BloodRequest> builder)
        {
            builder.ToTable("BloodRequests");

            builder.HasKey(br => br.Id);

            builder.Property(br => br.UrgencyLevel)
                .HasMaxLength(10);

            builder.Property(br => br.Status)
                .IsRequired()
                .HasDefaultValue(RequestStatus.Pending);

            builder.HasOne(br => br.Hospital)
                .WithMany(h => h.BloodRequests)
                .HasForeignKey(br => br.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(br => br.BloodType)
                .WithMany(bt => bt.BloodRequests)
                .HasForeignKey(br => br.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
