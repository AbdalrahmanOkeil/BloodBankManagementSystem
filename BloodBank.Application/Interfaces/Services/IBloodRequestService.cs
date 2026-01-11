using BloodBank.Application.Common;
using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IBloodRequestService
    {
        Task<Result<BloodRequestResponseDTO>> CreateAsync(string userId, CreateBloodRequestDTO dto);
        Task<Result<bool>> ApproveAsync(Guid requestId);
        Task<Result<bool>> RejectAsync(Guid requestId);
        Task<Result<List<BloodRequestResponseDTO>>> GetMyRequestsAsync(string userId);
    }
}
