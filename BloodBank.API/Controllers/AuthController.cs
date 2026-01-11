using BloodBank.API.Extensions;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloodBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registerDonor")]
        public async Task<IActionResult> RegisterDonorAsync([FromBody] RegisterDonorDTO registerDonorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterDonorAsync(registerDonorDTO);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        [HttpPost("registerHospital")]
        public async Task<IActionResult> RegisterHospitalAsync([FromBody] RegisterHospitalDTO registerHospitalDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterHospitalAsync(registerHospitalDTO);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDTO);

            if (!string.IsNullOrEmpty(result.Value.RefreshToken))
                SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);

            SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDTO revokeTokenDTO)
        {
            var token = revokeTokenDTO.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _authService.RevokeTokenAsync(token);

            return result.ToApiResponse();
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
