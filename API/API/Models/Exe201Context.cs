using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Exe201Context : DbContext
{
    public Exe201Context()
    {
    }

    public Exe201Context(DbContextOptions<Exe201Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDailyUsage> UserDailyUsages { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<VideoComment> VideoComments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =103.179.185.40; database = exe201;uid=sa;pwd=Hoancute1@;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC27BD990619");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA869BB7D8");

            entity.Property(e => e.CommentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("CommentID");
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EntityId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("EntityID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Entity).WithMany(p => p.CommentsNavigation)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("FK__Comments__Entity__403A8C7D");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__UserID__398D8EEE");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__C050D897C1BD7DC7");

            entity.HasIndex(e => e.UserId, "IDX_Conversations_UserID");

            entity.Property(e => e.ConversationId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ConversationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastMessageAt).HasColumnType("datetime");
            entity.Property(e => e.ModelUsed)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Conversat__UserI__693CA210");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Likes__A2922CF4AB5BA346");

            entity.Property(e => e.LikeId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("LikeID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EntityId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("EntityID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Entity).WithMany(p => p.LikesNavigation)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("FK__Likes__EntityID__412EB0B6");

            entity.HasOne(d => d.EntityNavigation).WithMany(p => p.LikesNavigation)
                .HasForeignKey(d => d.EntityId)
                .HasConstraintName("FK_Likes_Video");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Likes__UserID__3E52440B");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C27D25DDA");

            entity.HasIndex(e => e.ConversationId, "IDX_Messages_ConversationID");

            entity.Property(e => e.MessageId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("MessageID");
            entity.Property(e => e.ConversationId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ConversationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("FK__Messages__Conver__6D0D32F4");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAFC17B78E9");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("OrderID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SpecificAddress).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.Username).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__UserID__49C3F6B7");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__OrderDet__135C314DCFC1BA6C");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.DetailId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("DetailID");
            entity.Property(e => e.CategoryName).HasMaxLength(120);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("OrderID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ProductID");
            entity.Property(e => e.ProductUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__4D94879B");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__4E88ABD4");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A5871AF9D32");

            entity.HasIndex(e => e.UserId, "IDX_Payments_UserID");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Completed");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payments__UserID__7F2BE32F");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126038540D910F");

            entity.Property(e => e.PostId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("PostID");
            entity.Property(e => e.Author).HasMaxLength(255);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateUser).HasMaxLength(36);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.Thumbnail).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdateUser).HasMaxLength(36);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.VideoUrl).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__CategoryI__34C8D9D1");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__UserID__2B3F6F97");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6EDFE81AF8E");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateUser).HasMaxLength(36);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ProductUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdateUser).HasMaxLength(36);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__Categor__300424B4");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACC97C3E50");

            entity.ToTable("User");

            entity.HasIndex(e => new { e.Username, e.Email }, "IX_Users_Username_Email");

            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Bio).HasMaxLength(150);
            entity.Property(e => e.BlockUntil).HasColumnType("datetime");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.CreateUser).HasMaxLength(36);
            entity.Property(e => e.DistrictId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DistrictID");
            entity.Property(e => e.DistrictName).HasMaxLength(225);
            entity.Property(e => e.Dob).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.ExpiryDateToken).HasColumnType("datetime");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .HasColumnName("GoogleID");
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.LastLoginIp)
                .HasMaxLength(255)
                .HasColumnName("LastLoginIP");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.ProvinceId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ProvinceID");
            entity.Property(e => e.ProvinceName).HasMaxLength(225);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.UpdateUser).HasMaxLength(36);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<UserDailyUsage>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("PK__UserDail__29B197C0A3C77269");

            entity.ToTable("UserDailyUsage");

            entity.HasIndex(e => new { e.UserId, e.UsageDate }, "IDX_UserDailyUsage_UserID_Date");

            entity.HasIndex(e => new { e.UserId, e.UsageDate }, "UQ_UserDate").IsUnique();

            entity.Property(e => e.UsageId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UsageID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MaxQuestionsPerDay).HasDefaultValue(10);
            entity.Property(e => e.QuestionCount).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserDailyUsages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserDaily__UserI__73BA3083");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__UserSubs__9A2B24BDCFC43BB2");

            entity.HasIndex(e => e.UserId, "IDX_UserSubscriptions_UserID");

            entity.Property(e => e.SubscriptionId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("SubscriptionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.MaxQuestions).HasDefaultValue(100);
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UsedQuestions).HasDefaultValue(0);
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserSubsc__UserI__7A672E12");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK__Videos__BAE5126A5E7027F5");

            entity.Property(e => e.VideoId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.CreateUser).HasMaxLength(36);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Duration).HasMaxLength(100);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdateUser).HasMaxLength(36);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.VideoUrl).HasMaxLength(500);

            entity.HasOne(d => d.Category).WithMany(p => p.Videos)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Videos__Category__10566F31");
        });

        modelBuilder.Entity<VideoComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__VideoCom__C3B4DFCA0EA14E5B");

            entity.Property(e => e.CommentId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ParentCommentId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.VideoId)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__VideoComm__Paren__0E6E26BF");

            entity.HasOne(d => d.User).WithMany(p => p.VideoComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VideoComm__UserI__0D7A0286");

            entity.HasOne(d => d.Video).WithMany(p => p.VideoComments)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VideoComm__Video__0C85DE4D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
