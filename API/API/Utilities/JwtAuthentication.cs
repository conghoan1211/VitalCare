using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API.Configurations;
using API.Models;
using API.ViewModels.Token;
using API.Common;
using API.Helper;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InstagramClone.Utilities
{
    public class JwtAuthentication
    {
        public string GenerateJwtToken(User? user, HttpContext httpContext)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigManager.gI().SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Username", user.Username ?? ""),
                    new Claim("UserID", user.UserId.ToString() ?? ""),
                    new Claim("Avatar", user.Avatar.ToString() ?? ""),
                    new Claim(ClaimTypes.Role, user.RoleId == (int)Role.Admin ? "Admin" : "User"),
                }),
                Expires = DateTime.UtcNow.AddMinutes(ConfigManager.gI().ExpiresInMinutes),
                Issuer = ConfigManager.gI().Issuer,
                Audience = ConfigManager.gI().Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            httpContext.Response.Cookies.Append("JwtToken", tokenHandler.WriteToken(token), new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(7) // Token sống trong 7 ngày
            });

            return tokenHandler.WriteToken(token);
        }

        public UserToken? ParseJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigManager.gI().SecretKey);

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = ConfigManager.gI().Issuer,
                    ValidAudience = ConfigManager.gI().Audience,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true // Đảm bảo kiểm tra thời gian hết hạn
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.FirstOrDefault(x => x.Type == "Username")?.Value;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserID")?.Value;
                var avatar = jwtToken.Claims.FirstOrDefault(x => x.Type == "Avatar")?.Value;
                var roleId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

                return new UserToken
                {
                    UserName = username,
                    UserID = userId.ToGuid(),
                    Avatar = avatar,
                    RoleID = roleId,
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return null;  
            }
        }

        public bool ValidateJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(ConfigManager.gI().SecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = ConfigManager.gI().Issuer,
                    ValidAudience = ConfigManager.gI().Audience
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerateRefreshToken(User? user, Exe201Context _context, HttpContext httpContext)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            if (user == null) throw new Exception("User not found");
            user.RefreshToken = refreshToken;
            user.ExpiryDateToken = DateTime.UtcNow.AddDays(7);
            user.LastLogin = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false, // Chúng ta tắt kiểm tra thời gian sống ở đây vì token đã hết hạn
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ConfigManager.gI().SecretKey))
                }, out var validatedToken);

                return principal;
            }
            catch
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
        }


    }
}
