using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public interface IAccountService
    {
        public Task<string> DoToggleActive(string usertoken, string userId);
        public Task<(string, List<UserListVM>?)> GetList(string usetoken);
        public Task<(string, List<UserListVM>?)> DoSearch(string query);
        public Task<(string msg, User? user)> GetById(string userID);
        public Task<string> EditRoleActive(string userId, int roleId);
        public Task<string> Delete (string userId);
        public Task<(string, EditAccountVM?)> GetEdit(string userId);
    }
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly IAmazonS3Service _s3Service;

        public AccountService(IMapper mapper, Exe201Context context, IAmazonS3Service s3Service)
        {
            _context = context;
            _mapper = mapper;
            _s3Service = s3Service;
        }
        public async Task<(string msg, User? user)> GetById(string userID)
        {
            if (userID == null) return ("Không tìm thấy user id.", null);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userID);
            if (user == null) return ("User not found", null);

            return (string.Empty, user);
        }

        public async Task<string> DoToggleActive(string usertoken, string userId)
        {
            if (userId.IsEmpty()) return "User ID is not valid!";

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return "User is not available!";

            user.IsActive = !user.IsActive;
            user.UpdateAt = DateTime.UtcNow;
            user.UpdateUser = usertoken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<string> EditRoleActive(string userId, int roleId)
        {
            if (userId.IsEmpty()) return "User ID is not valid!";

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return "User is not available!";

            user.IsActive = !user.IsActive;
            user.RoleId = roleId;
            user.UpdateAt = DateTime.UtcNow;
            user.UpdateUser = userId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<(string, EditAccountVM?)> GetEdit(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return ("No User available", null);

            var mapper = _mapper.Map<EditAccountVM>(user);
            return ("", mapper);
        }

        public async Task<(string, List<UserListVM>?)> GetList(string usertoken)
        {
            var list = await _context.Users.Where(x => x.UserId != usertoken).ToListAsync();
            if (list.IsNullOrEmpty()) return ("No User available", null);

            var listMapper = _mapper.Map<List<UserListVM>>(list);
            return ("", listMapper);
        }

        public async Task<(string, List<UserListVM>?)> DoSearch(string query)
        {
            var list = await _context.Users
                .Where(x=> x.Username.ToLower().Contains(query.ToLower()))
                .ToListAsync();
            if (list.IsNullOrEmpty()) return ("No User available", null);

            var listMapper = _mapper.Map<List<UserListVM>>(list);
            return ("", listMapper);
        }

        public async Task<string> Delete(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return "User not found";

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            if (user.Avatar != null)                 // Xóa ảnh đã upload lên S3 nếu có lỗi
            {
                string key = $"{UrlS3.Profile}{user.UserId}";
                await _s3Service.DeleteFolderAsync(key);
            }
            return "";
        }

      
    }
}
