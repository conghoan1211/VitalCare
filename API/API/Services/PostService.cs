using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using InstagramClone.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace API.Services
{
    public interface IPostService
    {
        public Task<(string, List<PostListVM>?)> GetList(int privacy);
        public Task<(string, PostDetailVM?)> GetDetail(string postId);
        public Task<string> InsertUpdatePost(InsertUpdatePost? input, string userId);
        public Task<string> DoChangePrivacy(string postId, int privacy);
        public Task<string> DoDeletePost(string postId);
        public Task<(string, List<PostListVM>?)> GetListPopular();
        public Task<string> UpdateViews(string postId);
    }

    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly IAmazonS3Service _s3Service;

        public PostService(IMapper mapper, Exe201Context context, IAmazonS3Service s3Service)
        {
            _mapper = mapper;
            _context = context;
            _s3Service = s3Service;
        }

        public async Task<(string, List<PostListVM>?)> GetList(int privacy)
        {
            var list = await _context.Posts.Include(x => x.Category)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Post && (privacy == -1 || x.Privacy == privacy))
                .OrderBy(x => Guid.NewGuid())
                .ToListAsync();
            if (list == null || !list.Any()) return ("No post available!", null);

            var postMapper = _mapper.Map<List<PostListVM>>(list);

            return ("", postMapper);
        }

        public async Task<(string, List<PostListVM>?)> GetListPopular()
        {
            var list = await _context.Posts.Include(x => x.Category)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Post && x.Privacy == (int)PostPrivacy.Public)
                .OrderByDescending(x => x.Views)
                .ThenByDescending(x => x.Likes)
                .Take(5)
                .ToListAsync();
            if (list == null || !list.Any()) return ("No post available!", null);

            var postMapper = _mapper.Map<List<PostListVM>>(list);
            return ("", postMapper);
        }

        public async Task<(string, PostDetailVM?)> GetDetail(string postId)
        {
            var post = await _context.Posts.Include(x => x.Category).Include(x => x.User)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Post)
                .Select(x => new PostDetailVM
                {
                    PostId = x.PostId,
                    Title = x.Title,
                    Author = x.Author,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    Likes = x.Likes,
                    Comments = x.Comments,
                    Views = x.Views,
                    Tags = x.Tags,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    Thumbnail = x.Thumbnail,
                    Username = x.User.Username,
                    Avatar = x.User.Avatar,
                    Privacy = x.Privacy
                })
                .FirstOrDefaultAsync(x => x.PostId == postId);
            if (post == null) return ("No post available!", null);

            return ("", post);
        }

        public async Task<string> InsertUpdatePost(InsertUpdatePost? input, string userId)
        {
            if (userId == null) return "User id is null";
            string thumbnailUrl = "";

            if (string.IsNullOrEmpty(input.PostId))
            {
                input.PostId = Guid.NewGuid().ToString();

                string key = $"{UrlS3.Post}{input.PostId}";
                thumbnailUrl = await _s3Service.UploadFileAsync(key, input.Thumbnail);
                var newPost = new Post
                {
                    PostId = input.PostId,
                    UserId = userId,
                    Title = input.Title,
                    IsComment = input.IsComment,
                    Author = input.Author,
                    CategoryId = input.CategoryId,
                    Comments = 0,
                    Likes = 0,
                    Shares = 0,
                    Views = 0,
                    PinTop = input.PinTop,
                    Tags = input.Tags,
                    Content = input.Content,
                    CreateUser = userId,
                    CreatedAt = DateTime.UtcNow,
                    Privacy = input.Privacy,
                    Thumbnail = thumbnailUrl ?? "",
                };
                await _context.Posts.AddAsync(newPost);
            }
            else
            {
                var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == input.PostId);
                if (post == null) return "No post available!";

                if (input.Thumbnail != null)
                {
                    if (post.Thumbnail != null)
                    {
                        string folderKey = $"{UrlS3.Post}{input.PostId}/";
                        await _s3Service.DeleteFolderAsync(folderKey);
                    }
                    string key = $"{UrlS3.Post}{input.PostId}";
                    thumbnailUrl = await _s3Service.UploadFileAsync(key, input.Thumbnail);
                    post.Thumbnail = thumbnailUrl;
                }

                post.Author = input.Author;
                post.CategoryId = input.CategoryId;
                post.Content = input.Content;
                post.Title = input.Title;
                post.UpdatedAt = DateTime.UtcNow;
                post.UpdateUser = userId;
                post.IsComment = input.IsComment;
                post.PinTop = input.PinTop;
                post.Privacy = input.Privacy;
                post.Tags = input.Tags;
                _context.Posts.Update(post);
            }
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<string> DoChangePrivacy(string postId, int privacy)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == postId);
            if (post == null) return "Post not available!";

            post.Privacy = privacy;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<(string, List<PostListVM>?)> DoSearch(string title)
        {
            var list = await _context.Posts.Include(x => x.Category)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Post && x.Privacy == (int)PostPrivacy.Public
                && (x.Title.ToLower().Contains(title.ToLower()) || x.Category.Name.ToLower().Contains(title.ToLower()))
                )
                .ToListAsync();
            if (list == null || !list.Any()) return ("No post available!", null);

            var postMapper = _mapper.Map<List<PostListVM>>(list);

            return ("", postMapper);
        }

        public async Task<string> DoDeletePost(string postId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == postId);
                if (post == null) return "Không tìm thấy bài viết";

                if (post.Thumbnail != null)
                {
                    string folderKey = $"{UrlS3.Post}{post.PostId}/";
                    await _s3Service.DeleteFolderAsync(folderKey);
                }

                _context.Posts.Remove(post);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Đã xảy ra lỗi: {ex.InnerException}";
            }
        }

        public async Task<string> UpdateViews(string postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == postId);
            if (post == null) return "Post not found!";
            post.Views++;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return "";
        }

    }
}
