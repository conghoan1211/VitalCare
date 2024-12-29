using API.Models;
using API.Services;
using InstagramClone.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace API.Configurations
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext with connection string from the configuration
            services.AddDbContext<Exe201Context>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MyDB")));

            services.AddScoped<JwtAuthentication>(); // Register JwtAuthentication
            services.AddScoped<IAuthenticateService, AuthenticateService>();

        }
    }
}