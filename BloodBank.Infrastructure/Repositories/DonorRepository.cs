using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly BloodBankDbContext _context;
        public DonorRepository(BloodBankDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Donor donor)
        {
            await _context.Donors.AddAsync(donor);
        }

        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Donors.AnyAsync(d => d.UserId == userId);
        }

        public async Task<Donor?> GetByIdAsync(Guid donorId)
        {
            return await _context.Donors
                .FirstOrDefaultAsync(d => d.Id == donorId);
        }

        public async Task<Donor?> GetByUserIdAsync(string userId)
        {
            return await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<Donor?> GetWithDetailsByUserIdAsync(string userId)
        {
            return await _context.Donors
                .Include(d => d.BloodType)
                .Include(d => d.Donations)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            _context.SaveChangesAsync();
        }
    }
}
