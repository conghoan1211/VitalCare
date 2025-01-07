using API.Common;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using InstagramClone.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface IPostService
    {
        public Task<(string, List<PostListVM>?)> GetList();
        public Task<(string, PostVM?)> GetDetail(string postId);
        public Task<string> InsertUpdatePost(InsertUpdatePost? input, string userId);
        public Task<string> DoChangePrivacy(string postId, int privacy);

    }

    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly Exe201Context _context;
        private readonly JwtAuthentication _jwtAuthen;

        public PostService(IMapper mapper, Exe201Context context, JwtAuthentication jwtAuthen)
        {
            _mapper = mapper;
            _context = context;
            _jwtAuthen = jwtAuthen;
        }

        public async Task<(string, List<PostListVM>?)> GetList()
        {
            var list = await _context.Posts.Include(x => x.Category)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Post && x.Privacy == (int)PostPrivacy.Public)
                .ToListAsync();
            if (list == null || !list.Any()) return ("No post available!", null);

            var postMapper = _mapper.Map<List<PostListVM>>(list);

            return ("", postMapper);
        }

        public async Task<(string, PostVM?)> GetDetail(string postId)
        {
            var post = await _context.Posts.Include(x => x.Category)
                .Where(x => x.Category.TypeObject == (int)TypeCateria.Post)
                .FirstOrDefaultAsync(x => x.PostId == postId);
            if (post == null) return ("No post available!", null);

            var postMapper = _mapper.Map<PostVM>(post);

            return ("", postMapper);
        }

        public async Task<string> InsertUpdatePost(InsertUpdatePost? input, string userId)
        {
            if (userId == null) return "User id is null";

            string? msg = "", thumbnailUrl = "";
            var file = input.Thumbnail;
            if (file != null)
            {
                (msg, thumbnailUrl) = await Utils.GetUrlImage(file);
                if (msg.Length > 0) return msg;
            }

            if (!input.PostId.IsEmpty())
            {
                var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == input.PostId);
                if (post == null) return "No post available!";

                post.Thumbnail = thumbnailUrl ?? "";
                post.Author = input.Author;
                post.CategoryId = input.CategoryId;
                post.Content = input.Content;
                post.Title = input.Title;
                post.UpdatedAt = DateTime.Now;
                post.UpdateUser = input.UpdateUser;
                post.IsComment = input.IsComment;
                post.PinTop = input.PinTop;
                post.Privacy = input.Privacy;
                post.Tags = input.Tags;
                post.VideoUrl = input.VideoUrl;

                _context.Posts.Update(post);
            }
            else
            {
                var newPost = new Post
                {
                    PostId = Guid.NewGuid().ToString(),
                    UserId = input.UserId,
                    Title = input.Title,
                    VideoUrl = input.VideoUrl,
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
                    CreateUser = input.CreateUser,
                    CreatedAt = DateTime.Now,
                    Privacy = input.Privacy,
                    Thumbnail = thumbnailUrl ?? "",
                };
                await _context.Posts.AddAsync(newPost);
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
                && (x.Title.RemoveMarkVNToLower().Contains(title.RemoveMarkVNToLower()) || x.Title.RemoveMarkVNToLower().Contains(x.Category.Name.RemoveMarkVNToLower()))
                )
                .ToListAsync();
            if (list == null || !list.Any()) return ("No post available!", null);

            var postMapper = _mapper.Map<List<PostListVM>>(list);

            return ("", postMapper);
        }

        //public async Task<string> DoDeletePost(string postId)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var post = await _context.Posts.FirstOrDefaultAsync(x => x.PostId == postId);
        //        if (post == null) return "Không tìm thấy bài viết";

        //        var imagePaths = post.PostImages.Select(img => Path.Combine(Directory.GetCurrentDirectory(), Constant.UrlImagePath, img.ImageUrl)).ToList();
        //        if (imagePaths.Any())
        //        {
        //            foreach (var imagePath in imagePaths)
        //            {
        //                if (File.Exists(imagePath))
        //                {
        //                    File.Delete(imagePath);
        //                }
        //            }
        //        }
        //        _context.Posts.Remove(post);

        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return $"Đã xảy ra lỗi: {ex.InnerException}";
        //    }
        //}
    }
}
