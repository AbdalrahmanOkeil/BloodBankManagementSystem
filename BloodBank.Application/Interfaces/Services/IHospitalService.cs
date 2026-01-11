using BloodBank.Application.Common;
using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IHospitalService
    {
        Task<Result<bool>> CompleteProfileAsync(string userId, CompleteHospitalProfileDTO dto);
        Task<Result<HospitalProfileDTO>> GetMyProfileAsync(string userId);
        Task<Result<bool>> UpdateMyProfileAsync(string userId, UpdateHospitalDTO dto);
    }
}
