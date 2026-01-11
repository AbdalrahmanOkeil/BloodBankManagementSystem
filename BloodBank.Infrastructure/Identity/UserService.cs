using BloodBank.Application.DTOs;
using BloodBank.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace BloodBank.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<UserDTO?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new UserDTO
            {
                FullName = user.FullName,
                Email = user.Email
            };
        }
    }
}
