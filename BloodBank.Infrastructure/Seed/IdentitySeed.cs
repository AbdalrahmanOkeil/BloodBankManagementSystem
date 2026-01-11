using BloodBank.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Infrastructure.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //create role
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            //admin user seed
            var adminEmail = "admin@bloodbank.com";
            var adminUser = await userManager.FindByNameAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
