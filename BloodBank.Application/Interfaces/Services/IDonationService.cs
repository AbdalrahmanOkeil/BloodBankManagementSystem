using BloodBank.Application.Common;
using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IDonationService
    {
        Task<Result<DonationResponseDTO>> CreateDonationAsync(string userId, CreateDonationDTO dto);
        Task<Result<bool>> ApproveDonationAsync(Guid donationId);
        Task<Result<bool>> RejectDonationAsync(Guid donationId);
        Task<Result<IEnumerable<DonationDTO>>> GetDonationsAsync(string userId);
    }
}
