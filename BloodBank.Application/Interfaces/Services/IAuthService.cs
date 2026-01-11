using BloodBank.Application.Common;
using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<AuthDTO>> RegisterDonorAsync(RegisterDonorDTO dto);
        Task<Result<AuthDTO>> RegisterHospitalAsync(RegisterHospitalDTO dto);
        Task<Result<AuthDTO>> LoginAsync(LoginDTO dto);
        Task<Result<AuthDTO>> RefreshTokenAsync(string token);
        Task<Result<bool>> RevokeTokenAsync(string token);
    }
}
