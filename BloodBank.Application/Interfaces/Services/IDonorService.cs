using BloodBank.Application.Common;
using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IDonorService
    {
        Task<Result<bool>> CreateProfileAsync(string userId, CreateDonorDTO dto);

        // Latter 
        //Task UpdateProfileAsync(string userId, CreateDonorDTO dto);
        Task<Result<DonorProfileDTO>> GetProfileAsync(string userId);
        Task<Result<bool>> UpdateDonationInfoAsync(Guid donorId);
    }
}
