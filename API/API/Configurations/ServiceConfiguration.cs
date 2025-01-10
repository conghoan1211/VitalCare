using Amazon.S3;
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

            services.AddSingleton<JwtAuthentication>(); 
            services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(
                  ConfigManager.gI().AWSAccessKey,
                  ConfigManager.gI().AWSSecretKey,
                  Amazon.RegionEndpoint.GetBySystemName(ConfigManager.gI().AWSRegion)
            ));
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IAmazonS3Service, AmazonS3Service>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IProfileService, ProfileService>();


        }
    }
}