using Amazon.S3;
using API.Models;
using API.RabbitMQ;
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
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IChatbotService, ChatbotService>();
            services.AddHttpClient<IChatbotService, ChatbotService>();
            services.AddHttpClient<IVideoService, VideoService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IDashboardService, DashboardService>();

            //RabbitMQ
            services.AddSingleton<OrderProducer>();
            services.AddHostedService<OrderConsumerHostedService>();
        }
    }
}