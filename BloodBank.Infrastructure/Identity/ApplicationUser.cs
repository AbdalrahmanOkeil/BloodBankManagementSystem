using BloodBank.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BloodBank.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        //public string UserType { get; set; }  // Admin, Donor, Hospital
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
