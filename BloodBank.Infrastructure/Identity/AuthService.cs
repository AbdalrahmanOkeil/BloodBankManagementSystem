using BloodBank.Application.Common;
using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Domain.Entities;
using BloodBank.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BloodBank.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly ILogger<AuthService> _logger;
        public AuthService(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _jwt = jwt.Value;
        }

        public async Task<Result<AuthDTO>> RegisterDonorAsync(RegisterDonorDTO dto)
        {
            _logger.LogInformation("Register donor attempt for {Email}", dto.Email);

            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            {
                _logger.LogWarning("Register donor failed: {Email} already exists", dto.Email);
                return Result<AuthDTO>.Failure("Email is already registered!");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return Result<AuthDTO>.Failure(errors);
            }

            await _userManager.AddToRoleAsync(user, "Donor");

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var authDto = new AuthDTO
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = roles.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };

            _logger.LogInformation("Donor {UserId} registered successfully", user.Id);

            return Result<AuthDTO>.Success(authDto);
        }

        public async Task<Result<AuthDTO>> RegisterHospitalAsync(RegisterHospitalDTO dto)
        {
            _logger.LogInformation("Register hospital attempt for {Email}", dto.Email);

            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            {
                _logger.LogWarning("Hospital registration failed for {Email}", dto.Email);
                return Result<AuthDTO>.Failure("Email is already registered!");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.Name,
                PhoneNumber = dto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return Result<AuthDTO>.Failure(string.Join(",", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Hospital");

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var authDto = new AuthDTO
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = roles.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };

            _logger.LogInformation("Hospital {UserId} registered successfully", user.Id);

            return Result<AuthDTO>.Success(authDto);
        }

        public async Task<Result<AuthDTO>> LoginAsync(LoginDTO dto)
        {
            var authDto = new AuthDTO();

            var user = await _userManager.FindByEmailAsync(dto.Email);

            _logger.LogInformation("User {Email} is trying to login", dto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                _logger.LogWarning("Login failed for {Email}", dto.Email);
                return Result<AuthDTO>.Failure("Email or password is incorrect!");
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authDto.IsAuthenticated = true;
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authDto.Email = user.Email;
            authDto.UserName = user.UserName;
            authDto.ExpiresOn = jwtSecurityToken.ValidTo;
            authDto.Roles = rolesList.ToList();
            authDto.Message = "Logined";

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activerefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authDto.RefreshToken = activerefreshToken.Token;
                authDto.RefreshTokenExpiration = activerefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authDto.RefreshToken = refreshToken.Token;
                authDto.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Result<AuthDTO>.Success(authDto);
        }

        public async Task<Result<AuthDTO>> RefreshTokenAsync(string token)
        {
            _logger.LogInformation("Refresh token attempt");

            var authDto = new AuthDTO();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                _logger.LogWarning("Refresh token failed: token not found");
                return Result<AuthDTO>.Failure("Invalid token");
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                _logger.LogWarning("Refresh token failed: inactive token");
                return Result<AuthDTO>.Failure("Inactive token");
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authDto.IsAuthenticated = true;
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authDto.Email = user.Email;
            authDto.UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authDto.Roles = roles.ToList();
            authDto.RefreshToken = newRefreshToken.Token;
            authDto.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            _logger.LogInformation("Refresh token succeeded for user {UserId}", user.Id);

            return Result<AuthDTO>.Success(authDto);
        }

        public async Task<Result<bool>> RevokeTokenAsync(string token)
        {
            _logger.LogInformation("Revoke token attempt");

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                _logger.LogWarning("Revoke token failed: token not found");
                return Result<bool>.Failure("Invalid token");
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                _logger.LogWarning("Revoke token failed: token already inactive");
                return Result<bool>.Failure("Token is already inactive");
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Refresh token revoked for user {UserId}", user.Id);

            return Result<bool>.Success(true);
        }





        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
