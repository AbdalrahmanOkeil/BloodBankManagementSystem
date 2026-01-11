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
    public class BloodRequestsController : ControllerBase
    {
        private readonly IBloodRequestService _bloodRequestService;
        public BloodRequestsController(IBloodRequestService bloodRequestService)
        {
            _bloodRequestService = bloodRequestService;
        }

        [Authorize(Roles = "Hospital")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBloodRequestDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bloodRequestService.CreateAsync(userId, dto);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Hospital")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bloodRequestService.GetMyRequestsAsync(userId);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _bloodRequestService.ApproveAsync(id);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _bloodRequestService.RejectAsync(id);

            return result.ToApiResponse();
        }
    }
}
