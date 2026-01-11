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
    [Authorize(Roles = "Hospital")]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalService _hospitalService;
        public HospitalController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }

        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile(CompleteHospitalProfileDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _hospitalService.CompleteProfileAsync(userId, dto);

            return result.ToApiResponse();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _hospitalService.GetMyProfileAsync(userId);

            return result.ToApiResponse();
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateHospitalDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _hospitalService.UpdateMyProfileAsync(userId, dto);

            return result.ToApiResponse();
        }
    }
}
