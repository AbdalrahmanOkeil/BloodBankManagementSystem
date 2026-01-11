using BloodBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodBank.Infrastructure.Persistence.Configurations
{
    public class BloodTypeConfiguration : IEntityTypeConfiguration<BloodType>
    {
        public void Configure(EntityTypeBuilder<BloodType> builder)
        {
            builder.ToTable("BloodTypes");

            builder.HasKey(bt => bt.Id);

            builder.Property(bt => bt.Name)
                .IsRequired()
                .HasMaxLength(5);

            builder.HasOne(bt => bt.Stock)
                .WithOne(s => s.BloodType)
                .HasForeignKey<BloodStock>(s => s.BloodTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
            new BloodType { Id = 1, Name = "A+" },
            new BloodType { Id = 2, Name = "A-" },
            new BloodType { Id = 3, Name = "B+" },
            new BloodType { Id = 4, Name = "B-" },
            new BloodType { Id = 5, Name = "AB+" },
            new BloodType { Id = 6, Name = "AB-" },
            new BloodType { Id = 7, Name = "O+" },
            new BloodType { Id = 8, Name = "O-" }
            );
        }
    }
}
