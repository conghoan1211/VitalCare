﻿namespace API.Helper
{
    public static class Constant
    {
        public static readonly string UrlImagePath = "wwwroot/img";
        public static readonly IList<string> IMAGE_EXTENDS = new List<string> { ".png", ".jpg", ".jpeg", ".svg" }.AsReadOnly();

    }

    public static class ConstMessage
    {
        public static readonly string ACCOUNT_UNVERIFIED = "Tài khoản chưa được xác thực.";
        public static readonly string EMAIL_EXISTED = "Email này đã tồn tại.";

        public static readonly string NOTIFY_LIKE_POST = "liked your post.";
        public static readonly string NOTIFY_COMMENT_POST = "commented: ";
        public static readonly string NOTIFY_LIKE_COMMENT = "liked your comment: ";
        public static readonly string NOTIFY_NEW_FOLLOW = "started following you.";
        public static readonly string NOTIFY_ACCEPTED_FRIENDS = "accepted your friends request.";

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

    public enum MessageStatus
    {
        NotSent = 0,
        Sent,
        Recieved,
        Read,
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
        Friend,
        Private,
    }

    public enum FriendStatus
    {
        Pending = 0,
        Accepted,
        Blocked,
    }

    public enum FollowStatus
    {
        Nothing = 0,
        Following =1,
        Followed =2,
        Friend =3,
        Block= 4,
        Mute,
    }


}