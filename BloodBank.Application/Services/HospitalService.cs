using BloodBank.Application.Common;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BloodBank.Application.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IUserService _userService;
        private readonly ILogger<HospitalService> _logger;

        public HospitalService(IHospitalRepository hospitalRepository,
            IUserService userService,
            ILogger<HospitalService> logger)
        {
            _hospitalRepository = hospitalRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Result<bool>> CompleteProfileAsync(string userId, CompleteHospitalProfileDTO dto)
        {
            _logger.LogInformation("Creating hospital profile for user {userId}", userId);

            var exists = await _hospitalRepository.GetByUserIdAsync(userId);
            if (exists is not null)
            {
                _logger.LogWarning("Hospital profile already exists for user {userId}", userId);
                return Result<bool>.Failure("Profile already exists!");
            }

            var hospital = new Hospital
            {
                UserId = userId,
                Name = dto.Name,
                Address = dto.Address,
            };

            await _hospitalRepository.AddAsync(hospital);
            await _hospitalRepository.SaveChangesAsync();

            _logger.LogInformation("Hospital profile created for user {userId}", userId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<HospitalProfileDTO>> GetMyProfileAsync(string userId)
        {
            _logger.LogInformation("Fetching hospital profile for user {userId}", userId);

            var hospital = await _hospitalRepository.GetByUserIdAsync(userId);
            if (hospital is null)
            {
                _logger.LogWarning("Hospital profile not found for user {userId}", userId);
                return Result<HospitalProfileDTO>.Failure("Hospital not found!");
            }

            var user = await _userService.GetUserByIdAsync(userId);

            var response = new HospitalProfileDTO
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = hospital.Address,
                PhoneNumber = user.PhoneNumber
            };

            _logger.LogInformation("Returned hospital profile for user {userId}", userId);

            return Result<HospitalProfileDTO>.Success(response);
        }

        public async Task<Result<bool>> UpdateMyProfileAsync(string userId, UpdateHospitalDTO dto)
        {
            _logger.LogInformation("Updating hospital profile for user {userId}", userId);

            var hospital = await _hospitalRepository.GetByUserIdAsync(userId);
            if (hospital is null)
            {
                _logger.LogWarning("Update failed: Hospital not found for user {userId}", userId);
                return Result<bool>.Failure("Hospital not found!");
            }

            hospital.Name = dto.Name;
            hospital.Address = dto.Address;

            var user = await _userService.GetUserByIdAsync(userId);
            user.PhoneNumber = dto.PhoneNumber;

            await _hospitalRepository.SaveChangesAsync();

            _logger.LogInformation("Hospital profile updated successfully for user {userId}", userId);

            return Result<bool>.Success(true);
        }
    }
}
