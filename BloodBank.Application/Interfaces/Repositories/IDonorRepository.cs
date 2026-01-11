using BloodBank.Domain.Entities;

namespace BloodBank.Application.Interfaces.Repositories
{
    public interface IDonorRepository
    {
        Task<bool> ExistsByUserIdAsync(string userId);
        Task AddAsync(Donor donor);
        Task<Donor?> GetByIdAsync(Guid donorId);
        Task<Donor?> GetByUserIdAsync(string userId);
        Task<Donor?> GetWithDetailsByUserIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
