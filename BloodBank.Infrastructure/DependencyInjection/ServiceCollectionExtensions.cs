using BloodBank.Application.Interfaces.Repositories;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Infrastructure.Identity;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Repositories;
using BloodBank.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloodBank.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<BloodBankDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IDonorRepository, DonorRepository>();
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<IBloodStockRepository, BloodStockRepository>();
            services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
            services.AddScoped<IHospitalRepository, HospitalRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDonorService, DonorService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<IBloodStockService, BloodStockService>();
            services.AddScoped<IBloodRequestService, BloodRequestService>();
            services.AddScoped<IHospitalService, HospitalService>();

            return services;
        }
    }
}
