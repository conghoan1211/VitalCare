using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace API.Services
{
    public interface IVideoService
    {
        public Task<(string, List<VideoListVM>?)> GetList(bool active, string order);
        public Task<(string, VideoDetailVM?)> GetDetail(string videoId);
        public Task<string> InsertUpdateVideo(InsertUpdateVideoVM input, string userId);
        public Task<string> DeleteVideo(string videoId);
        public Task<string> UpdateViews(string videoId);
        public Task<(string, int?)> UpdateLikes(string videoId);
        public Task<(string, int?)> UpdateDisLikes(string videoId);
        public Task<string> ChangeActive(string videoId, string userId);
        public Task<(string, List<CategoryVideoVM>?)> GetCategory();
    }
    public class VideoService : IVideoService
    {
        private readonly Exe201Context _context;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public VideoService(IMapper mapper, Exe201Context context, HttpClient httpClient)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<(string, List<VideoListVM>?)> GetList(bool active, string order)
        {
            var list = await _context.Videos.Include(x => x.Category)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Video && (active == false || x.IsActive == active))
                .ToListAsync();

            if (list == null || !list.Any()) return ("No video available!", null);
            list = order switch
            {
                "created_dec" => list.OrderByDescending(x => x.CreatedAt).ToList(),
                "view_dec" => list.OrderByDescending(x => x.Views).Take(5).ToList(),
                _ => list
            };
            var videoMapper = _mapper.Map<List<VideoListVM>>(list);

            return ("", videoMapper);
        }

        public async Task<(string, VideoDetailVM?)> GetDetail(string videoId)
        {
            var video = await _context.Videos.Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.VideoId == videoId);

            if (video == null) return ("Video not found!", null);
            var videoMapper = _mapper.Map<VideoDetailVM>(video);
            return ("", videoMapper);
        }
        public async Task<(string, List<CategoryVideoVM>?)> GetCategory()
        {
            var cates = await _context.Categories.Where(x => x.IsDeleted == false && x.IsActive == true
                      && x.TypeObject == (int)TypeCateria.Video).ToListAsync();
            if(cates == null || !cates.Any()) return ("No category available!", null);
            
            var cateMapper = _mapper.Map<List<CategoryVideoVM>>(cates);
            return ("", cateMapper);
        }

        public async Task<string> InsertUpdateVideo(InsertUpdateVideoVM input, string userId)
        {
            if (userId == null) return "Usertoken not found!";
            if (input.VideoId.IsEmpty())
            {
                var newVideo = new Video
                {
                    VideoId = Guid.NewGuid().ToString(),
                    Author = input.Author,
                    CategoryId = input.CategoryId,
                    CreatedAt = DateTime.UtcNow,
                    Duration = input.Duration,
                    IsActive = input.IsActive,
                    Title = input.Title,
                    VideoUrl = input.VideoUrl,
                    Description = input.Description,
                    CreateUser = userId,
                    Likes = 0,
                    Views = 0,
                };
                await _context.Videos.AddAsync(newVideo);
            }
            else
            {
                var oldVideo = await _context.Videos.FirstOrDefaultAsync(x => x.VideoId == input.VideoId);
                if (oldVideo == null) return "Video not found!";

                oldVideo.Author = input.Author;
                oldVideo.CategoryId = input.CategoryId;
                oldVideo.Duration = input.Duration;
                oldVideo.IsActive = input.IsActive;
                oldVideo.Title = input.Title;
                oldVideo.VideoUrl = input.VideoUrl;
                oldVideo.Description = input.Description;
                oldVideo.UpdatedAt = DateTime.UtcNow;
                oldVideo.UpdateUser = userId;

                _context.Videos.Update(oldVideo);
            }
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> DeleteVideo(string videoId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null) return "Video not found!";
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> UpdateViews(string videoId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null) return "Video not found!";
            video.Views++;
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<(string, int?)> UpdateLikes(string videoId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null) return ("Video not found!", 0);
            video.Likes++;
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
            return ("", video.Likes);
        }
        public async Task<(string, int?)> UpdateDisLikes(string videoId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null) return ("Video not found!", 0);
            video.Likes--;
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
            return ("", video.Likes);
        }

        public async Task<string> ChangeActive(string videoId, string userId)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.VideoId == videoId);
            if (video == null) return "Video not found!";
            video.IsActive = !video.IsActive;
            video.UpdatedAt = DateTime.UtcNow;
            video.UpdateUser = userId;
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
            return "";
        }

    }

}