using BloodBank.API.Extensions;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Donor")]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _donorService;
        private readonly IDonationService _donationService;
        public DonorsController(IDonorService donorService, IDonationService donationService)
        {
            _donorService = donorService;
            _donationService = donationService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donorService.GetProfileAsync(userId);

            return result.ToApiResponse();
        }

        [HttpPost("me/complete-profile")]
        public async Task<IActionResult> CompleteMyProfile(CreateDonorDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donorService.CreateProfileAsync(userId, dto);

            return result.ToApiResponse();
        }

        [HttpGet("me/donations")]
        public async Task<IActionResult> GetMyDonations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _donationService.GetDonationsAsync(userId);

            return result.ToApiResponse();
        }
    }
}
