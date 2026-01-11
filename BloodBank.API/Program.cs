using BloodBank.API.Middlewares;
using BloodBank.Application.Interfaces.Services;
using BloodBank.Infrastructure.DependencyInjection;
using BloodBank.Infrastructure.Helpers;
using BloodBank.Infrastructure.Identity;
using BloodBank.Infrastructure.Persistence;
using BloodBank.Infrastructure.Seed;
using BloodBank.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace BloodBank.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        shared: true);
            });

            // Add services to the container.

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<BloodBankDbContext>();

            builder.Services.AddInfrastructure(builder.Configuration);

            

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<GlobalExceptionMiddleware>();


            app.MapControllers();

            await IdentitySeed.SeedAsync(app);

            app.Run();
        }
    }
}
