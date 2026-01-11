using BloodBank.API.Extensions;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodStockController : ControllerBase
    {
        private readonly IBloodStockService _bloodStockService;
        public BloodStockController(IBloodStockService bloodStockService)
        {
            _bloodStockService = bloodStockService;
        }

        [Authorize(Roles = "Admin,Hospital,Donor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bloodStockService.GetAllAsync();
            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin,Hospital,Donor")]
        [HttpGet("{bloodTypeId}")]
        public async Task<IActionResult> GetByBloodType(int bloodTypeId)
        {
            var result = await _bloodStockService.GetByBloodTypeIdAsync(bloodTypeId);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin,Hospital")]
        [HttpPut("{bloodTypeId}/increase")]
        public async Task<IActionResult> IncreaseStock(int bloodTypeId, UpdateStockDTO dto)
        {
            var result = await _bloodStockService.IncreaseAsync(bloodTypeId, dto.Units);

            return result.ToApiResponse();
        }

        [Authorize(Roles = "Admin,Hospital")]
        [HttpPut("{bloodTypeId}/decrease")]
        public async Task<IActionResult> DecreaseStock(int bloodTypeId, UpdateStockDTO dto)
        {
            var result = await _bloodStockService.DecreaseAsync(bloodTypeId, dto.Units);

            return result.ToApiResponse();
        }
    }
}
