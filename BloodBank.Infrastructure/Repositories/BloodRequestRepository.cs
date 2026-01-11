using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class BloodRequestRepository : IBloodRequestRepository
    {
        private readonly BloodBankDbContext _context;
        public BloodRequestRepository(BloodBankDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(BloodRequest request)
        {
            await _context.BloodRequests.AddAsync(request);
        }

        public async Task<List<BloodRequest>> GetByHospitalIdAsync(Guid hospitalId)
        {
            return await _context.BloodRequests
                .Include(r => r.BloodType)
                .Where(r => r.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<BloodRequest?> GetByIdAsync(Guid id)
        {
            return await _context.BloodRequests
                .Include(r => r.BloodType)
                .Include(r => r.Hospital)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
