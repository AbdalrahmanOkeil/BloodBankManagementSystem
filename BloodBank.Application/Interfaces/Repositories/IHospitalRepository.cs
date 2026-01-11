using BloodBank.Domain.Entities;

namespace BloodBank.Application.Interfaces.Repositories
{
    public interface IHospitalRepository
    {
        Task<Hospital?> GetByUserIdAsync(string userId);
        Task<Hospital?> GetByIdAsync(Guid id);
        Task AddAsync(Hospital hospital);
        Task SaveChangesAsync();
    }
}
