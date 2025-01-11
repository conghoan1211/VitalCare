using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using API.Configurations;
using API.Models;
using API.ViewModels.Token;
using API.Common;
using API.Helper;
using Microsoft.AspNetCore.Http;

namespace InstagramClone.Utilities
{
    public class JwtAuthentication
    {
        public string GenerateJwtToken(User? user, HttpContext httpContext)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(ConfigManager.gI().SecretKey);  

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Username", user.Username ?? ""),
                    new Claim("UserID", user.UserId.ToString() ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
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
                SameSite = SameSiteMode.Strict // Prevent cross-site attacks
            });

            return tokenHandler.WriteToken(token);
        }

        public string ParseCurrentToken(ClaimsPrincipal user, out UserToken userToken)
        {
            userToken = null;
            if (user.Identity is not ClaimsIdentity claimsIdentity || !user.Identity.IsAuthenticated)
            {
                return "User has not authenticated";
            }
            var claims = claimsIdentity.Claims;
            var username = claims.FirstOrDefault(c => c.Type == "Username")?.Value;
            var userID = claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var roleID = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            userToken = new UserToken
            {
                UserName = username,
                UserID = userID.ToGuid(),
                Email = email,
                RoleID = roleID,
            };
            return "";
        }

        public UserToken ParseJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigManager.gI().SecretKey); // Your encryption key for token validation

            // Validate the token
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero, // Disable the default 5 min leeway
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Extract user information from the token claims
            var username = jwtToken.Claims.FirstOrDefault(x => x.Type == "Username")?.Value;
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserID")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var roleId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            // Create a UserToken object with the extracted data
            var authUser = new UserToken
            {
                UserName = username,
                UserID = userId.ToGuid(),
                Email = email,
                RoleID = roleId,
            };

            return authUser;
        }
    }
}
