using BloodBank.Application.Common;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BloodBank.Infrastructure.Services
{
    public class BloodStockService : IBloodStockService
    {
        private readonly IBloodStockRepository _bloodStockRepository;
        private readonly ILogger<BloodStockService> _logger;
        public BloodStockService(IBloodStockRepository bloodStockRepository, ILogger<BloodStockService> logger)
        {
            _bloodStockRepository = bloodStockRepository;
            _logger = logger;
        }

        public async Task<Result<List<BloodStockResponseDTO>>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all blood stock");

            var stocks = await _bloodStockRepository.GetAllAsync();

            var response = stocks.Select(s => new BloodStockResponseDTO
            {
                Id = s.Id,
                BloodType = s.BloodType.Name,
                UnitsAvailable = s.UnitsAvailable
            }).ToList();

            _logger.LogInformation("Returned {Count} blood stock records", response.Count);

            return Result<List<BloodStockResponseDTO>>.Success(response);
        }

        public async Task<Result<BloodStockResponseDTO>> GetByBloodTypeIdAsync(int bloodTypeId)
        {
            _logger.LogInformation("Fetching blood stock for bloodTypeId {bloodTypeId}", bloodTypeId);

            var stock = await _bloodStockRepository.GetByBloodTypeIdAsync(bloodTypeId);
            if (stock is null)
            {
                _logger.LogWarning("Blood stock not found for bloodTypeId {bloodTypeId}", bloodTypeId);
                return Result<BloodStockResponseDTO>.Failure("Blood stock not found!");
            }

            var response = new BloodStockResponseDTO
            {
                Id = stock.Id,
                BloodType = stock.BloodType.Name,
                UnitsAvailable = stock.UnitsAvailable
            };

            return Result<BloodStockResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> IncreaseAsync(int bloodTypeId, int units)
        {
            _logger.LogInformation("Increasing blood stock. BloodTypeId={bloodTypeId}", bloodTypeId);

            await _bloodStockRepository.IncreaseStockAsync(bloodTypeId, units);
            await _bloodStockRepository.SaveChangesAsync();

            _logger.LogInformation("Blood stock increased successfully. BloodTypeId={bloodTypeId}", bloodTypeId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DecreaseAsync(int bloodTypeId, int units)
        {
            _logger.LogInformation("Decreasing blood stock. BloodTypeId={bloodTypeId}", bloodTypeId);

            var stock = await _bloodStockRepository.GetByBloodTypeIdAsync(bloodTypeId);
            if (stock is null)
            {
                _logger.LogWarning("Decrease failed: stock not found for BloodTypeId {bloodTypeId}", bloodTypeId);
                return Result<bool>.Failure("Blood stock not found!");
            }

            if (stock.UnitsAvailable < units)
            {
                _logger.LogWarning(
                    "Decrease failed: insufficient stock for BloodTypeId {bloodTypeId}", bloodTypeId);
                return Result<bool>.Failure("Insufficient blood stock!");
            }

            await _bloodStockRepository.DecreaseStockAsync(bloodTypeId, units);
            await _bloodStockRepository.SaveChangesAsync();

            _logger.LogInformation("Blood stock decreased successfully. BloodTypeId={bloodTypeId}", bloodTypeId);

            return Result<bool>.Success(true);
        }
    }
}
