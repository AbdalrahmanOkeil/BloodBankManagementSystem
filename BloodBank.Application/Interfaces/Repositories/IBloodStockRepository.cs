using BloodBank.Domain.Entities;

namespace BloodBank.Application.Interfaces.Repositories
{
    public interface IBloodStockRepository
    {
        Task<List<BloodStock>> GetAllAsync();
        Task<BloodStock?> GetByBloodTypeIdAsync(int bloodTypeId);
        Task IncreaseStockAsync(int bloodTypeId, int units);
        Task DecreseStockAsync(int bloodTypeId, int units);
        Task SaveChangesAsync();
    }
}
