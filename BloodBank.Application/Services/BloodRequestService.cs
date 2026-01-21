using BloodBank.Application.Common;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Domain.Entities;
using BloodBank.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BloodBank.Application.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IBloodStockRepository _bloodStockRepository;
        private readonly ILogger<BloodRequestService> _logger;
        public BloodRequestService(IBloodRequestRepository bloodRequestRepository,
            IHospitalRepository hospitalRepository,
            IBloodStockRepository bloodStockRepository,
            ILogger<BloodRequestService> logger)
        {
            _bloodRequestRepository = bloodRequestRepository;
            _hospitalRepository = hospitalRepository;
            _bloodStockRepository = bloodStockRepository;
            _logger = logger;
        }
        public async Task<Result<BloodRequestResponseDTO>> CreateAsync(string userId, CreateBloodRequestDTO dto)
        {
            _logger.LogInformation("Hospital {userId} is creating blood request", userId);

            var hospital = await _hospitalRepository.GetByUserIdAsync(userId);
            if (hospital is null)
            {
                _logger.LogWarning("Blood request failed: Hospital not found for user {userId}", userId);
                return Result<BloodRequestResponseDTO>.Failure("Hospital not found!");
            }

            var request = new BloodRequest
            {
                HospitalId = hospital.Id,
                BloodTypeId = dto.BloodTypeId,
                UnitsRequested = dto.Units,
                Status = RequestStatus.Pending,
                UrgencyLevel = dto.UrgencyLevel,
                CreatedAt = DateTime.UtcNow,
            };

            await _bloodRequestRepository.AddAsync(request);

            var stock = await _bloodStockRepository.GetByBloodTypeIdAsync(dto.BloodTypeId);
            if (stock is not null && stock.UnitsAvailable >= dto.Units)
            {
                _logger.LogInformation("Blood request {requestId} auto-approved.", request.Id);
                stock.UnitsAvailable -= dto.Units;
                request.Status = RequestStatus.Approved;
                request.ApprovedAt = DateTime.UtcNow;
            }

            if(request.Status == RequestStatus.Pending)
                _logger.LogInformation("Blood request {requestId} is pending. Not enough stock.", request.Id);

            await _bloodRequestRepository.SaveChangesAsync();

            var response = new BloodRequestResponseDTO
            {
                Id = request.Id,
                BloodType = request.BloodType.Name,
                Units = request.UnitsRequested,
                Status = request.Status.ToString(),
                RequestedAt = request.CreatedAt
            };

            return Result<BloodRequestResponseDTO>.Success(response);
        }

        public async Task<Result<bool>> ApproveAsync(Guid requestId)
        {
            _logger.LogInformation("Approving blood request {requestId}", requestId);

            var request = await _bloodRequestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogWarning("Approve failed: request {requestId} not found", requestId);
                return Result<bool>.Failure("Blood request not found!");
            }

            if (request.Status != RequestStatus.Pending)
            {
                _logger.LogWarning("Approve failed: request {requestId} already processed", requestId);
                return Result<bool>.Failure("Request already processed");
            }

            var stock = await _bloodStockRepository.GetByBloodTypeIdAsync(request.BloodTypeId);
            if (stock is null || stock.UnitsAvailable < request.UnitsRequested)
            {
                _logger.LogWarning("Approve failed: insufficient stock for request {requestId}", requestId);
                return Result<bool>.Failure("Not enough units in stock");
            }

            stock.UnitsAvailable -= request.UnitsRequested;
            request.Status = RequestStatus.Approved;
            request.ApprovedAt = DateTime.UtcNow;

            await _bloodRequestRepository.SaveChangesAsync();

            _logger.LogInformation("Blood request {requestId} approved successfully", requestId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RejectAsync(Guid requestId)
        {
            _logger.LogInformation("Rejecting blood request {requestId}", requestId);

            var request = await _bloodRequestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogWarning("Approve failed: request {requestId} not found", requestId);
                return Result<bool>.Failure("Blood request not found!");
            }

            if (request.Status != RequestStatus.Pending)
            {
                _logger.LogWarning("Approve failed: request {requestId} already processed", requestId);
                return Result<bool>.Failure("Request already processed");
            }

            request.Status = RequestStatus.Rejected;

            await _bloodRequestRepository.SaveChangesAsync();

            _logger.LogInformation("Blood request {requestId} rejected", requestId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<List<BloodRequestResponseDTO>>> GetMyRequestsAsync(string userId)
        {
            _logger.LogInformation("Hospital {userId} is fetching blood requests", userId);

            var hospital = await _hospitalRepository.GetByUserIdAsync(userId);
            if (hospital is null)
            {
                _logger.LogWarning("Get requests failed: Hospital not found for user {userId}", userId);
                return Result<List<BloodRequestResponseDTO>>.Failure("Hospital not found!");
            }

            var requests = await _bloodRequestRepository.GetByHospitalIdAsync(hospital.Id);

            var response = requests.Select(r => new BloodRequestResponseDTO
            {
                Id = r.Id,
                BloodType = r.BloodType.Name,
                Units = r.UnitsRequested,
                Status = r.Status.ToString(),
                RequestedAt = r.CreatedAt
            }).ToList();

            return Result<List<BloodRequestResponseDTO>>.Success(response);
        }      
    }
}
