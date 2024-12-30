using API.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// add config manager appsettings
builder.Services.ConfigureServices(builder.Configuration);
ConfigManager.CreateManager(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));


// Add session 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian chờ phiên
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Chắc chắn cookie có mặt
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>  // JWT Bearer
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = ConfigManager.gI().Issuer,
        ValidAudience = ConfigManager.gI().Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigManager.gI().SecretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HttpContext.Response.Cookies.Delete("JwtToken");
            context.Response.Redirect("/explore"); // login
            context.HandleResponse();
            await Task.CompletedTask;
        }
    };
})
.AddGoogle(googleOptions =>  // Google OAuth
{
    googleOptions.ClientId = ConfigManager.gI().GoogleClientIp;
    googleOptions.ClientSecret = ConfigManager.gI().GoogleClientSecert;
    googleOptions.CallbackPath = new PathString(ConfigManager.gI().GoogleRedirectUri); 
    googleOptions.SaveTokens = true;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000") // Replace with your frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Set-Cookie");  // Thêm dòng này

        });
});


var app = builder.Build();
app.UseSession();
app.UseCors("AllowAll");
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin");
    context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
