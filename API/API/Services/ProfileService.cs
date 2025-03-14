using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using InstagramClone.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface IProfileService
    {
        public Task<(string msg, bool success)> ToggleIsActive(string userId);
        public Task<(string msg, ProfileVM? result)> GetProfile(string userID);
        public Task<(string, UpdateProfileModels?)> GetProfileUpdate(string userID);
        public Task<(string, string?)> DoChangeAvatar(string userid, UpdateAvatarVM input, HttpContext http);
        public Task<string> UpdateProfile( UpdateProfileModels? updatedProfile, HttpContext http);
    }
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly JwtAuthentication _jwtAuthen;
        private readonly IAmazonS3Service _s3Service;

        public ProfileService(IMapper mapper, Exe201Context context, JwtAuthentication jwtAuthen, IAmazonS3Service s3Service)
        {
            _context = context;
            _jwtAuthen = jwtAuthen;
            _mapper = mapper;
            _s3Service = s3Service;
        }

        public async Task<(string msg, bool success)> ToggleIsActive(string userId)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null) return ("Người dùng không tồn tại.", false);

                existingUser.IsActive = !existingUser.IsActive;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return ("Cập nhật trạng thái thành công.", true);
            }
            catch (Exception ex)
            {
                return ($"Lỗi khi cập nhật trạng thái: {ex.Message}", false);
            }
        }

        public async Task<(string msg, ProfileVM? result)> GetProfile(string userID)
        {
            if (userID == null) return ("Không tìm thấy user id.", null);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userID);
            if (user == null) return ("User not found", null);

            var profile = _mapper.Map<ProfileVM>(user);

            return (string.Empty, profile);
        }

        public async Task<string> UpdateProfile( UpdateProfileModels? updatedProfile, HttpContext http)
        {
            if (updatedProfile == null) return "Invalid profile data";
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == updatedProfile.UserID);
            if (user == null) return "User not found";

            user.Sex = updatedProfile.Sex;
            user.Dob = updatedProfile.Dob;
            user.Username = updatedProfile.UserName;
            user.UpdateAt = DateTime.UtcNow;
            user.Address = updatedProfile.Address;
            user.ProvinceId = updatedProfile.ProvinceId;
            user.DistrictId = updatedProfile.DistrictId;
            user.Email = updatedProfile.Email;
            user.Phone = updatedProfile.Phone;
            user.DistrictName = updatedProfile.DistrictName;
            user.ProvinceName = updatedProfile.ProvinceName;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 🔥 Chỉ cập nhật token nếu có HttpContext (tức là gọi từ HTTP request)
            if (http != null)
            {
                http.Response.Cookies.Delete("JwtToken");
                var token = _jwtAuthen.GenerateJwtToken(user, http);
            }
            return "";
        }

        public async Task<(string, UpdateProfileModels?)> GetProfileUpdate(string userID)
        {
            if (userID == null) return ("Không tìm thấy user id.", null);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userID);
            if (user == null) return ("User not found", null);
        
            var profile = _mapper.Map<UpdateProfileModels>(user);
            return ("", profile);
        }

        public async Task<(string, string?)> DoChangeAvatar(string userid, UpdateAvatarVM input, HttpContext http)
        {
            try
            {
                if (userid is null ) return ("User ID is null", null);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userid);
                if (user == null) return ("User not found", null);
                if (input.Image == null) return ("File ảnh không hợp lệ!", null);

                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    string oldAvatarUrl = Helper.Common.ExtractKeyFromUrl(user.Avatar);
                    if (!string.IsNullOrEmpty(oldAvatarUrl))
                    {
                        await _s3Service.DeleteFileAsync(oldAvatarUrl);
                    }
                    string key = $"{UrlS3.Profile}{userid}";
                    await _s3Service.DeleteFolderAsync(key);
                }
                string newAvatarKey = $"{UrlS3.Profile}{userid}/{input.Image.FileName}";
                string url = await _s3Service.UploadFileAsync(newAvatarKey, input.Image);
                if (url.IsEmpty()) return ("Error: Cannot upload file to s3!", null);

                user.Avatar = url;
                _context.Users.Update(user);

                await _context.SaveChangesAsync();
                // 🔥 Chỉ cập nhật token nếu có HttpContext (tức là gọi từ HTTP request)
                if (http != null)
                {
                    http.Response.Cookies.Delete("JwtToken");
                    var token = _jwtAuthen.GenerateJwtToken(user, http);
                }
                return ("", url);
            }
            catch (Exception ex)
            {
                return ($"An error occurred while updating the avatar: {ex.Message}", null);
            }
        }
    }
}
