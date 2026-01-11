using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Persistence
{
    public class BloodBankDbContext : IdentityDbContext<ApplicationUser>
    {
        public BloodBankDbContext(DbContextOptions<BloodBankDbContext> options) : base(options)
        {
        }
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(BloodBankDbContext).Assembly);
        }
    }
}
