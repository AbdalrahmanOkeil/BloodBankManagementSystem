using BloodBank.Domain.Entities;

namespace BloodBank.Application.Interfaces.Repositories
{
    public interface IDonationRepository
    {
        Task AddAsync(Donation donation);
        Task<Donation?> GetByIdAsync(Guid donationId);
        Task<IEnumerable<Donation>> GetByDonorIdAsync(Guid donorId);
        Task SaveChangesAsync();
    }
}
