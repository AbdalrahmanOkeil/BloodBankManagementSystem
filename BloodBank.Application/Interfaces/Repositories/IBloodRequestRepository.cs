using BloodBank.Domain.Entities;

namespace BloodBank.Application.Interfaces.Repositories
{
    public interface IBloodRequestRepository
    {
        Task AddAsync(BloodRequest request);
        Task<BloodRequest?> GetByIdAsync(Guid id);
        Task<List<BloodRequest>> GetByHospitalIdAsync(Guid hospitalId);
        Task SaveChangesAsync();
    }
}
