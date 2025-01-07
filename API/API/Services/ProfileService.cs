﻿using API.Common;
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
        public Task<string> DoChangeAvatar(string userid, UpdateAvatarVM input, HttpContext http);

        public Task<string> UpdateProfile(string userID, UpdateProfileModels? updatedProfile, HttpContext http);
    }
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly JwtAuthentication _jwtAuthen;
        public ProfileService(IMapper mapper, Exe201Context context, JwtAuthentication jwtAuthen)
        {
            _context = context;
            _jwtAuthen = jwtAuthen;
            _mapper = mapper;
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
            if (userID == null)   return ("Không tìm thấy user id.", null);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userID);
            if (user == null) return ("User not found", null);

            var profile = _mapper.Map<ProfileVM>(user);

            return (string.Empty, profile);
        }

        public async Task<string> UpdateProfile(string userID, UpdateProfileModels? updatedProfile, HttpContext http)
        {
            if (updatedProfile == null) return "Invalid profile data";
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userID);
            if (user == null) return "User not found";

            var oldProfile = new UpdateProfileModels
            {
                UserName = user.Username, Sex = user.Sex,  Dob = user.Dob,  Bio = user.Bio
            };

            if (oldProfile.IsObjectEqual(updatedProfile)) return "";

            user.Sex = updatedProfile.Sex;
            user.Dob = updatedProfile.Dob;
            user.Bio = updatedProfile.Bio;
            user.Username = updatedProfile.UserName;
            user.UpdateAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            http.Response.Cookies.Delete("JwtToken");        
            var token = _jwtAuthen.GenerateJwtToken(user, http);
            return "";
        }

        public async Task<(string, UpdateProfileModels?)> GetProfileUpdate(string userID)
        {
            var user = await _context.Users.Where(x => x.UserId == userID)
                .Select(u => new UpdateProfileModels
                {
                    UserName = u.Username,
                    Bio = u.Bio,
                    Dob = u.Dob,
                    Sex = u.Sex,
                }).FirstOrDefaultAsync();
            if (user == null) return ("User not found", null);
            return ("", user);
        }

        public async Task<string> DoChangeAvatar(string userid, UpdateAvatarVM input, HttpContext http)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userid);
            if (user == null) return "User not found";

            var files = input.Image;
            if (files == null)  return "";

            var (msg, fileName) = await Utils.GetUrlImage(files);
            if (msg.Length > 0) return msg;

            var oldAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), Constant.UrlImagePath, user.Avatar ?? "");
            if (File.Exists(oldAvatarPath)) File.Delete(oldAvatarPath);   // delete old file

            try
            {
                user.Avatar = fileName;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Update the JWT token
                http.Response.Cookies.Delete("JwtToken"); 
                var token = _jwtAuthen.GenerateJwtToken(user, http); 
        
            }
            catch (Exception ex)
            {
                return $"An error occurred while updating the avatar: {ex.Message}";
            }
            return "";
        }
    }
}