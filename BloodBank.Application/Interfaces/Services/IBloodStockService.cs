using BloodBank.Application.Common;
using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IBloodStockService
    {
        Task<Result<List<BloodStockResponseDTO>>> GetAllAsync();
        Task<Result<BloodStockResponseDTO>> GetByBloodTypeIdAsync(int bloodTypeId);
        Task<Result<bool>> IncreaseAsync(int bloodTypeId, int units);
        Task<Result<bool>> DecreaseAsync(int bloodTypeId, int units);
    }
}
