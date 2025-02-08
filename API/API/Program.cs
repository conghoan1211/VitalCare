using API.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
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
builder.Services.AddHttpClient();

// Cấu hình OData
builder.Services.AddControllers().AddOData(options =>
    options.Select().Filter().OrderBy().Expand().SetMaxTop(100).Count());
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
            //   context.Response.Redirect("http://localhost:3000/login");  
            //      context.HandleResponse();
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
}).AddFacebook(facebookOptions =>  // Facebook OAuth
{
    facebookOptions.AppId = ConfigManager.gI().FacebookAppId;
    facebookOptions.AppSecret = ConfigManager.gI().FacebookAppSecret;
    facebookOptions.CallbackPath = new PathString(ConfigManager.gI().FacebookRedirectUri);
    facebookOptions.SaveTokens = true;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")  // Đổi thành domain frontend
                  .AllowCredentials() // Quan trọng để cookie hoạt động
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("Set-Cookie");
        });
});

// Đảm bảo rằng cookie không bị chặn trong ứng dụng hoặc trình duyệt
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;

});

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

builder.Services.AddAuthorization(options =>              //  Add Authentication
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("UserWithClaim", policy =>
        policy.RequireClaim("CustomClaim", "AllowedValue"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token "
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseCors("AllowFrontend");

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin");
//    context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
//    await next();
//});

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:3000" );
    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
    await next();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"message\":\"Bạn không có quyền sử dụng tính năng của admin\"}");
    }
});
app.MapControllers();

await app.RunAsync();
