﻿namespace API.Configurations
{
    public class ConfigManager
    {
        private static ConfigManager _instance;
        private readonly IConfiguration _configuration;

        public string EmailDisplayName { get; set; }
        public string EmailHost { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }

        // JWT settings
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
      
        // Google settings
        public string GoogleClientIp{ get; set; }
        public string GoogleClientSecert{ get; set; }
        public string GoogleRedirectUri { get; set; }

        public static ConfigManager gI()
        {
            return _instance;
        }

        public static void CreateManager(IConfiguration configuration)
        {
            _instance = null;
            _instance = new ConfigManager(configuration);
        }

        public ConfigManager(IConfiguration configuration)
        {
            _configuration = configuration;
            Init();
        }

        public void Init()
        {
            try
            {
                EmailDisplayName = _configuration.GetSection("EmailSettings").GetSection("EmailDisplayName").Value;
                EmailHost = _configuration.GetSection("EmailSettings").GetSection("EmailHost").Value;
                EmailUsername = _configuration.GetSection("EmailSettings").GetSection("EmailUsername").Value;
                EmailPassword = _configuration.GetSection("EmailSettings").GetSection("EmailPassword").Value;
                SecretKey = _configuration.GetSection("JwtSettings").GetSection("SecretKey").Value;
                Issuer = _configuration.GetSection("JwtSettings").GetSection("SecretKey").Value;
                Audience = _configuration.GetSection("JwtSettings").GetSection("Audience").Value;
                ExpiresInMinutes = int.Parse(_configuration.GetSection("JwtSettings").GetSection("ExpiresInMinutes").Value);
                GoogleClientIp = _configuration.GetSection("Google").GetSection("ClientId").Value;
                GoogleClientSecert = _configuration.GetSection("Google").GetSection("ClientSecret").Value;
                GoogleRedirectUri = _configuration.GetSection("Google").GetSection("RedirectUri").Value;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}; InnerException: {e.InnerException}");
                EmailDisplayName = "Interactive Messaging Online Platform";
                EmailHost = "smtp.gmail.com";
                EmailUsername = "hoanpche170404@fpt.edu.vn";
                EmailPassword = "icfswakmfidrsfgi";
            }
        }
    }
}
