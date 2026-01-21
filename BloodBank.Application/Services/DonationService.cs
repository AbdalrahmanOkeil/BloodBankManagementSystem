using BloodBank.Application.Common;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Domain.Entities;
using BloodBank.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BloodBank.Application.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IBloodStockRepository _bloodStockRepository;
        private readonly ILogger<DonationService> _logger;
        public DonationService(
            IDonationRepository donationRepository,
            IDonorRepository donorRepository,
            IBloodStockRepository bloodStockRepository,
            ILogger<DonationService> logger)
        {
            _donationRepository = donationRepository;
            _donorRepository = donorRepository;
            _bloodStockRepository = bloodStockRepository;
            _logger = logger;
        }
        public async Task<Result<bool>> ApproveDonationAsync(Guid donationId)
        {
            //using var transaction = await _context.Database.BeginTransactionAsync();
            _logger.LogInformation("Approving donation {donationId}", donationId);

            var donation = await _donationRepository.GetByIdAsync(donationId);

            if (donation is null)
            {
                _logger.LogWarning("Approve Failed: Donation {donationId} no found", donationId);
                return Result<bool>.Failure("Donation not found!");
            }

            if (donation.Status != DonationStatus.Pending)
            {
                _logger.LogWarning("Approve failed: Donation {donationId} already {Status}", donationId, donation.Status);
                return Result<bool>.Failure("Donation already processed!");
            }

            var donor = donation.Donor;

            if (donor.LastDonationDate.HasValue &&
                DateTime.UtcNow.Subtract(donor.LastDonationDate.Value).Days < 90)
            {
                _logger.LogWarning("Approve failed: Donor {donorId} not eligible (last donation {LastDonation})", donor.Id, donor.LastDonationDate);
                return Result<bool>.Failure("Donor not eligible!");
            }

            donation.Status = DonationStatus.Approved;

            donor.LastDonationDate = DateTime.UtcNow;
            donor.TotalDonations++;

            await _bloodStockRepository.IncreaseStockAsync(donation.BloodTypeId, donation.UnitsDonated);

            await _bloodStockRepository.SaveChangesAsync();
            //await transaction.CommitAsync();

            _logger.LogInformation("Donation {donationId} approved.", donation.Id);

            return Result<bool>.Success(true);
        }

        public async Task<Result<DonationResponseDTO>> CreateDonationAsync(string userId, CreateDonationDTO dto)
        {
            _logger.LogInformation("User {userId} is trying to create a donation", userId);

            var donor = await _donorRepository.GetByUserIdAsync(userId);

            if (donor == null)
            {
                _logger.LogWarning("Donation failed: Donor not found for user {userId}", userId);
                return Result<DonationResponseDTO>.Failure("Donor not found!");
            }

            if (donor.LastDonationDate.HasValue &&
                DateTime.UtcNow.Subtract(donor.LastDonationDate.Value).Days < 90)
            {
                _logger.LogWarning("User {userId} is not eligible for donation", userId);
                return Result<DonationResponseDTO>.Failure("Dononr is not eligible yet!");
            }

            var donation = new Donation
            {
                DonorId = donor.Id,
                BloodTypeId = dto.BloodTypeId,
                UnitsDonated = dto.Units,
                Status = DonationStatus.Pending,
                DonationDate = DateTime.UtcNow
            };

            await _donationRepository.AddAsync(donation);
            await _donationRepository.SaveChangesAsync();

            var response = new DonationResponseDTO
            {
                Id = donation.Id,
                DonationDate = donation.DonationDate,
                Units = donation.UnitsDonated,
                Status = donation.Status.ToString()
            };

            _logger.LogInformation("Donation created successfully for user {userId}", userId);

            return Result<DonationResponseDTO>.Success(response);
        }

        public async Task<Result<IEnumerable<DonationDTO>>> GetDonationsAsync(string userId)
        {
            _logger.LogInformation("Fetching donations for user {userId}", userId);

            var donor = await _donorRepository.GetByUserIdAsync(userId);

            if (donor is null)
            {
                _logger.LogWarning("Donations fetch failed: Donor not found for user {userId}", userId);
                return Result<IEnumerable<DonationDTO>>.Failure("Donor not found!");
            }

            var donations = await _donationRepository.GetByDonorIdAsync(donor.Id);
            var response = donations.Select(d => new DonationDTO
            {
                Id = d.Id,
                DonationDate = d.DonationDate,
                UnitsDonated = d.UnitsDonated,
                Status = d.Status,
                BloodType = d.BloodType.Name
            });

            _logger.LogInformation("Returned {Count} donations", donations.Count());

            return Result<IEnumerable<DonationDTO>>.Success(response);
        }

        public async Task<Result<bool>> RejectDonationAsync(Guid donationId)
        {
            _logger.LogInformation("Rejecting donation {donationId}", donationId);

            var donation = await _donationRepository.GetByIdAsync(donationId);

            if (donation is null)
            {
                _logger.LogWarning("Reject failed: Donation {donationId} not found", donationId);
                return Result<bool>.Failure("Donation not found!");
            }

            donation.Status = DonationStatus.Rejected;
            await _donationRepository.SaveChangesAsync();

            _logger.LogInformation("Donation {donationId} rejected", donationId);

            return Result<bool>.Success(true);
        }
    }
}
