using API.Configurations;
using API.Services;

namespace API.Helper
{
    public static class Constant
    {
        public static readonly string UrlImagePath = "wwwroot/img";
        public static readonly IList<string> IMAGE_EXTENDS = new List<string> { ".png", ".jpg", ".jpeg" }.AsReadOnly();

        public const long AVATAR_FILE_SIZE = 1 * 1024 * 1024;
    }

    public static class ConstMessage
    {
        public static readonly string ACCOUNT_UNVERIFIED = "Tài khoản chưa được xác minh.";
        public static readonly string EMAIL_EXISTED = "Email này đã tồn tại.";

        public static readonly string NOTIFY_LIKE_POST = "liked your post.";
        public static readonly string NOTIFY_COMMENT_POST = "commented: ";
        public static readonly string NOTIFY_LIKE_COMMENT = "liked your comment: ";
        public static readonly string NOTIFY_NEW_FOLLOW = "started following you.";
        public static readonly string NOTIFY_ACCEPTED_FRIENDS = "accepted your friends request.";

        public static readonly string CHATAI_DEFAULT_MODEL = "gpt-3.5-turbo";
        public static readonly string CONVERSATION_DEFAULT_TITLE = "Cuộc trò chuyện mới";
    }

    public static class UrlS3
    {
        public static readonly string UrlMain = ConfigManager.gI().UrlS3Key;
        public static readonly string Profile = "profile/";
        public static readonly string Product = "product/";
        public static readonly string Post = "post/";

    }

    public enum NotifyType
    {
        FriendRequest = 0,
        FriendAccept,
        PostLike,
        PostComment,
        CommentLike,
        Message
    }

    public enum OrderStatus
    {
        Pending = 0, // 0 - Chờ xác nhận
        Processing,  // 1 - Đang xử lý
        Delivering,  // 2 - Đang giao
        Completed,   // 3 - Đã giao
        Cancelled    // 4 - Đã hủy
    }

    public enum ProductStatus
    {
        Available = 0,
        OutOfStock,
    }

    public enum UserStatus
    {
        Inactive = 0, // Người dùng không hoạt động
        Active,   // Người dùng đang hoạt động
        Banned,   // Người dùng bị cấm
        Suspended // Người dùng bị đình chỉ
    }

    public enum Role
    {
        User = 0,
        Admin,
    }

    public enum ChatbotRole
    {
        User = 0,
        Assistant,
        System,
    }

    public enum TypeCateria
    {
        Product = 1,
        Post,
        Video,
    }

    public enum Gender
    {
        Male = 0,
        Female,
        Other,
    }

    public enum PostPrivacy
    {
        Public = 0,
        Private,
    }


}
