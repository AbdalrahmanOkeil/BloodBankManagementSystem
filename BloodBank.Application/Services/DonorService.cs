using BloodBank.Application.Common;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BloodBank.Infrastructure.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        private readonly IUserService _userService;
        private readonly ILogger<DonorService> _logger;
        public DonorService(IDonorRepository donorRepository, IUserService userService, ILogger<DonorService> logger)
        {
            _donorRepository = donorRepository;
            _userService = userService;
            _logger = logger;
        }
        public async Task<Result<bool>> CreateProfileAsync(string userId, CreateDonorDTO dto)
        {
            _logger.LogInformation("Creating donor profile for user {userId}", userId);

            if (await _donorRepository.ExistsByUserIdAsync(userId))
            {
                _logger.LogWarning("Create profile failed: Donor profile already exists for user {userId}", userId);
                return Result<bool>.Failure("Donor profile already exists!");
            }

            var donor = new Donor
            {
                UserId = userId,
                BloodTypeId = dto.BloodTypeId,
                DateOfBirth = dto.DateOfBirth,
                HealthStatus = dto.HealthStatus,
                LastDonationDate = null,
                TotalDonations = 0
            };

            await _donorRepository.AddAsync(donor);
            await _donorRepository.SaveChangesAsync();

            _logger.LogInformation("Donor profile created successfully for user {userId}", userId);

            return Result<bool>.Success(true);
        }

        public async Task<Result<DonorProfileDTO>> GetProfileAsync(string userId)
        {
            _logger.LogInformation("Fetching donor profile for user {userId}", userId);

            var donor = await _donorRepository.GetWithDetailsByUserIdAsync(userId);

            if (donor == null)
            {
                _logger.LogWarning("Donor not found for user {userId}", userId);
                return Result<DonorProfileDTO>.Failure("Donor not found!");
            }

            var user = await _userService.GetUserByIdAsync(userId);

            var response = new DonorProfileDTO
            {
                Id = donor.Id,
                FullName = user.FullName,
                Email = user.Email,
                BloodType = donor.BloodType.Name,
                HealthStatus = donor.HealthStatus,
                TotalDonations = donor.TotalDonations,
                LastDonationDate = donor.LastDonationDate,
                IsEligible =
                    donor.LastDonationDate == null ||
                    DateTime.UtcNow.Subtract(donor.LastDonationDate.Value).Days >= 90
            };

            _logger.LogInformation("Returned donor profile for user {userId}", userId);

            return Result<DonorProfileDTO>.Success(response);
        }

        public async Task<Result<bool>> UpdateDonationInfoAsync(Guid donorId)
        {
            _logger.LogInformation("Updating donation info for donor {donorId}", donorId);

            var donor = await _donorRepository.GetByIdAsync(donorId);

            if (donor == null)
            {
                _logger.LogWarning("Update donation info failed: Donor {donorId} not found", donorId);
                return Result<bool>.Failure("Donor not found!");
            }

            donor.LastDonationDate = DateTime.Now;
            donor.TotalDonations++;

            await _donorRepository.SaveChangesAsync();

            _logger.LogInformation("Donation info updated for donor {DonorId}", donorId);

            return Result<bool>.Success(true);
        }

        //public async Task UpdateProfileAsync(string userId, CreateDonorDTO dto)
        //{
        //    var donor = await _donorRepository.GetByUserIdAsync(userId);
        //    if (donor is null)
        //        throw new Exception("Donor not found!");

        //    donor.DateOfBirth = dto.DateOfBirth;
        //    donor.HealthStatus = dto.HealthStatus;
        //    donor.DateOfBirth = dto.DateOfBirth;

        //    await _context.SaveChangesAsync();
        //}
    }
}
