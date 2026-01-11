using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BloodBank.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;
        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<UserDTO?> GetUserByIdAsync(string userId)
        {
            _logger.LogInformation("Start fetching user with ID {userId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {userId} not found", userId);
                return null;
            }

            _logger.LogInformation("User with ID {userId} found: {email}", userId, user.Email);

            return new UserDTO
            {
                FullName = user.FullName,
                Email = user.Email
            };
        }
    }
}
