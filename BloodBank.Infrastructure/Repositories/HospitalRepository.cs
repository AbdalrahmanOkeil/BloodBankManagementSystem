using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly BloodBankDbContext _context;
        public HospitalRepository(BloodBankDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Hospital hospital)
        {
            await _context.Hospitals.AddAsync(hospital);
        }

        public async Task<Hospital?> GetByIdAsync(Guid id)
        {
            return await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<Hospital?> GetByUserIdAsync(string userId)
        {
            return await _context.Hospitals
                .FirstOrDefaultAsync(h => h.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            _context.SaveChangesAsync();
        }
    }
}
