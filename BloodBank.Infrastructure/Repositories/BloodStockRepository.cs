using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class BloodStockRepository : IBloodStockRepository
    {
        private readonly BloodBankDbContext _context;
        public BloodStockRepository(BloodBankDbContext context)
        {
            _context = context;
        }
        public async Task DecreseStockAsync(int bloodTypeId, int units)
        {
            var stock = await GetByBloodTypeIdAsync(bloodTypeId);

            if (stock is null || stock.UnitsAvailable < units)
                throw new Exception("Insufficient blood stock!");

            stock.UnitsAvailable -= units;
        }

        public async Task<List<BloodStock>> GetAllAsync()
        {
            return await _context.BloodStocks
                .Include(bs => bs.BloodType)
                .ToListAsync();
        }

        public async Task<BloodStock?> GetByBloodTypeIdAsync(int bloodTypeId)
        {
            return await _context.BloodStocks
                .Include(bs=>bs.BloodType)
                .FirstOrDefaultAsync(b => b.BloodTypeId == bloodTypeId);
        }

        public async Task IncreaseStockAsync(int bloodTypeId, int units)
        {
            var stock = await GetByBloodTypeIdAsync(bloodTypeId);

            if (stock is null)
            {
                stock = new BloodStock
                {
                    BloodTypeId = bloodTypeId,
                    UnitsAvailable = units
                };

                await _context.BloodStocks.AddAsync(stock);
            }
            else
            {
                stock.UnitsAvailable += units;
            }
        }

        public async Task SaveChangesAsync()
        {
             await _context.SaveChangesAsync();
        }
    }
}
