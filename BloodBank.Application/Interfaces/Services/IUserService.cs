using BloodBank.Application.DTOs;

namespace BloodBank.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByIdAsync(string userId);
    }
}
