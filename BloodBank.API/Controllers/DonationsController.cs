using BloodBank.API.Extensions;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationService _donationService;
        public DonationsController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [Authorize(Roles = "Donor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDonationDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _donationService.CreateDonationAsync(userId, dto);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _donationService.ApproveDonationAsync(id);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _donationService.RejectDonationAsync(id);

            return result.ToApiResponse();
        }
    }
}
