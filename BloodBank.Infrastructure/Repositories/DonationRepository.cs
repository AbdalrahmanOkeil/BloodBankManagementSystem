using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class DonationRepository : IDonationRepository
    {
        private readonly BloodBankDbContext _context;
        public DonationRepository(BloodBankDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Donation donation)
        {
            await _context.Donations.AddAsync(donation);
        }

        public async Task<IEnumerable<Donation>> GetByDonorIdAsync(Guid donorId)
        {
            return await _context.Donations
                .Where(d => d.DonorId == donorId)
                .Include(d => d.BloodType)
                .ToListAsync();
        }

        public async Task<Donation?> GetByIdAsync(Guid donationId)
        {
            return await _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.BloodType)
                .FirstOrDefaultAsync(d => d.Id == donationId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
