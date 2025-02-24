--Create database exe201


CREATE TABLE [User](
	[UserID] [varchar](36) Primary key NOT NULL ,
	[Username] [nvarchar](255) NULL,
	[Phone] [nvarchar](10) NULL,
	[Email] [nvarchar](255) NULL,
	[Password] [nvarchar](255) NULL,
	[Avatar] [nvarchar](255) NULL, 
	[RoleID] [int] NULL,
	[GoogleID] [nvarchar](255) NULL,
	[Sex] [int] NULL,
	[Dob] [datetime] NULL,
	[Bio] [nvarchar](150) NULL,
	[Address] [nvarchar](255) NULL,
	[ProvinceID] [varchar](20) NULL,
	[DistrictID] [varchar](20) NULL,
	[DistrictName] [nvarchar](225) NULL,
	[ProvinceName] [nvarchar](225) NULL,
	[IsDisable] [bit] NULL,    -- turn off by user
	[IsActive] [bit] NULL,      -- turn off by admin
	[IsVerified] [bit] NULL,
	[CreateAt] [datetime] NULL,
	[UpdateAt] [datetime] NULL,
	[CreateUser] [nvarchar](36) NULL,
	[UpdateUser] [nvarchar](36) NULL,
	[Status] [int] NULL,                -- status online
	[BlockUntil] [datetime] NULL,
	[LastLogin] [datetime] NULL,
	[LastLoginIP] [nvarchar](255) NULL,
    [RefreshToken] [varchar](255) NULL,
    [ExpiryDateToken] [datetime] NULL,
)

CREATE TABLE [Posts](
	[PostID] [varchar](36) primary key NOT NULL ,
	[UserID] [varchar](36) NOT NULL,
	[CategoryID] [int] NOT NULL, 
	[Content] [nvarchar](MAX) NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Thumbnail] [nvarchar](255) NOT NULL,
	[VideoUrl] [nvarchar](255) NULL,
	[Privacy] [int] NULL,
	[Tags] [nvarchar](255) NULL,
	[Author] [nvarchar](255) NULL,
	[Likes] [int] NULL,
	[Comments] [int] NULL,
	[Shares] [int] NULL,
	[Views] [int] NULL,
	[IsComment] [bit] NULL,
	[PinTop] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreateUser] [nvarchar](36) NULL,
	[UpdateUser] [nvarchar](36) NULL,
	FOREIGN KEY([UserID]) REFERENCES [User] ([UserID]),
	FOREIGN KEY([CategoryID]) REFERENCES [Category] ([ID])
)

CREATE TABLE Category (
    [ID] [int] PRIMARY KEY IDENTITY(1,1),
    [Name] [nvarchar](255),
	[TypeObject] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
);

CREATE TABLE Product (
	[ProductID] [varchar](36) PRIMARY KEY,
    [Title] [nvarchar](255) NOT NULL,
    [CurrentPrice] [int] NOT NULL,
    [NewPrice] [int] NULL,
    [ImageUrl] [nvarchar](MAX) NULL,
    [ProductUrl] [nvarchar](500) NULL,
    [Description] [nvarchar](MAX) NULL,
    [CategoryID] [int] NOT NULL, 
	[Sold] [int] NULL,
    [Stock] [int] NULL,        
    [Status] [int] NULL,      -- yeu thich/ het hang/ khuyen mai/ sale...	 	
	[Rating] FLOAT NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
    [CreateUser] [nvarchar](36) NULL,
 	[UpdateUser] [nvarchar](36) NULL,
	FOREIGN KEY ([CategoryID]) REFERENCES Category(ID),
);

CREATE TABLE [Comments](
	[CommentID] [varchar](36) primary key NOT NULL,
	[EntityID] [varchar](36) NOT NULL,
	[UserID] [varchar](36) NOT NULL,
	[TypeObject] [int] NOT NULL,                 -- 1: post, 2: product
	[Content] [nvarchar](500) NULL,
	[Likes] [int] NULL,
	[IsHide] [int] NULL,
	[IsPinTop] [bit] NULL,
	[CreatedAt] [datetime] NULL,
    FOREIGN KEY([UserID]) REFERENCES [dbo].[User] ([UserID]),
    FOREIGN KEY([EntityID]) REFERENCES [dbo].[Posts] ([PostID])  ON DELETE CASCADE,
)

CREATE TABLE  [Likes](
	[LikeID] [varchar](36) primary key NOT NULL,
	[UserID] [varchar](36) NOT NULL,
	[EntityID] [varchar](36) NOT NULL,
	[TypeObject] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,

    FOREIGN KEY([EntityID]) REFERENCES [dbo].[Posts] ([PostID])  ON DELETE CASCADE,
    FOREIGN KEY([UserID] ) REFERENCES [dbo].[User] ([UserID])
)

CREATE TABLE [Order] (
	[OrderID] [varchar](36) primary key NOT NULL,
	[UserID] [varchar](36) NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[Phone] [varchar](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[SpecificAddress] [nvarchar](255) NOT NULL,
	[Note] [nvarchar](255) NOT NULL,
	[OrderDate] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedAt] [datetime] NULL,
	[Status] [int] NOT NULL,  -- Trạng thái đơn hàng: 0 - Chờ xác nhận, 1 - Đang xử lý, 2 - Đang giao, 3 - Đã giao, 4 - Đã hủy
	[PaymentMethod] [int] NOT NULL,  
	[Total] [int] NOT NULL,
	[ShipPrice] [int] NOT NULL,
	 FOREIGN KEY ([UserID]) REFERENCES [User]([UserID])
)

CREATE TABLE [OrderDetail] (
	[DetailID] [varchar](36) primary key NOT NULL,
	[OrderID] [varchar](36) NOT NULL,
	[ProductID] [varchar](36) NOT NULL ,
	[Title] [nvarchar](255) NOT NULL,
    [CurrentPrice] [int] NOT NULL,
    [NewPrice] [int] NULL,
    [ProductUrl] [nvarchar](500) NULL,
    [CategoryName] [nvarchar](120) NOT NULL, 
    [Quantity] [int] NOT NULL,   
    [Total] [int] NOT NULL,
    [CreateAt] [datetime] DEFAULT GETDATE(),
    [UpdateAt] [datetime] NULL,
    FOREIGN KEY ([OrderID]) REFERENCES [Order]([OrderID]) ON DELETE CASCADE,
    FOREIGN KEY ([ProductID]) REFERENCES Product([ProductID])
);

-- Conversations table to store chat conversations
CREATE TABLE Conversations (
    ConversationID [varchar](36) primary key NOT NULL,
    UserID [varchar](36) NOT NULL,
    Title [nvarchar](255) NOT NULL,
    IsActive [bit] DEFAULT 1,
	[CreatedAt] [datetime] DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    LastMessageAt [datetime],
    ModelUsed [varchar](50), -- e.g., 'gpt-4', 'claude-3', etc.
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Messages table to store individual messages in conversations
CREATE TABLE [Messages] (
    MessageID [varchar](36) primary key NOT NULL,
    ConversationID [varchar](36) NOT NULL,
    [Role] [int] NOT NULL, -- 'user', 'assistant', or 'system'
    Content [nvarchar](MAX) NOT NULL,
	[CreatedAt] [datetime] DEFAULT GETDATE(),
    FOREIGN KEY (ConversationID) REFERENCES Conversations(ConversationID) ON DELETE CASCADE
);

-- Usage statistics table for tracking API usage
CREATE TABLE UserDailyUsage (
    UsageID [varchar](36) PRIMARY KEY NOT NULL,
    UserID [varchar](36) NOT NULL,
    UsageDate [date] NOT NULL,  -- Lưu ngày sử dụng
    QuestionCount [int] DEFAULT 0,  -- Số câu hỏi user đã gửi trong ngày
    MaxQuestionsPerDay [int] DEFAULT 10, -- Số câu hỏi tối đa trong ngày (có thể điều chỉnh)
    CreatedAt [datetime] DEFAULT GETDATE(),
    UpdatedAt [datetime] NULL,
    CONSTRAINT UQ_UserDate UNIQUE (UserID, UsageDate), -- Đảm bảo mỗi user chỉ có một bản ghi mỗi ngày
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

--Quản lý số lượng câu hỏi theo tháng.
CREATE TABLE UserSubscriptions (
    SubscriptionID [varchar](36) PRIMARY KEY NOT NULL,
    UserID [varchar](36) NOT NULL,
    StartDate [datetime] DEFAULT GETDATE(), -- Ngày bắt đầu gói
    ExpiryDate [datetime] NOT NULL,  -- Ngày hết hạn gói (1 tháng sau)
    MaxQuestions [int] DEFAULT 100, -- Số câu hỏi tối đa trong tháng
    UsedQuestions [int] DEFAULT 0, -- Số câu hỏi đã dùng trong tháng
    CreatedAt [datetime] DEFAULT GETDATE(),
    UpdatedAt [datetime] NULL,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

CREATE TABLE Payments (
    PaymentID [varchar](36) PRIMARY KEY NOT NULL,
    UserID [varchar](36) NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL, -- Số tiền nạp vào
    PaymentDate [datetime] DEFAULT GETDATE(),
    [Status] [varchar](20) DEFAULT 'Completed', -- 'Completed', 'Pending', 'Failed'
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

CREATE TABLE Videos (
    VideoId [varchar](36) PRIMARY KEY NOT NULL,
    Title [nvarchar](255) NOT NULL,
    [Description] [nvarchar](MAX) NULL,
    VideoUrl [nvarchar](500) NOT NULL,
	Author [nvarchar](100) NOT NULL,
    ThumbnailUrl [nvarchar](500) NULL, -- Ảnh thumbnail
    Duration [int] NULL, -- Thời lượng video (giây)
    CategoryId [varchar](36) NULL, -- Danh mục video
	[Views] [int] NULL,
	[Likes] [int] NULL,
    CreatedAt [datetime] DEFAULT GETDATE(),
    UpdatedAt [datetime]  ,
);

CREATE TABLE VideoComments (
    CommentId [varchar](36) PRIMARY KEY NOT NULL,
    VideoId [varchar](36) NOT NULL,
    UserId [varchar](36) NOT NULL,
    Content [nvarchar](500) NOT NULL,
    ParentCommentId  [varchar](36) NULL, -- NULL nếu là bình luận gốc, có giá trị nếu là reply
    CreatedAt [datetime] DEFAULT GETDATE(),
    FOREIGN KEY (VideoId) REFERENCES Videos(VideoId),
    FOREIGN KEY (UserId) REFERENCES [User](UserID),
    FOREIGN KEY (ParentCommentId) REFERENCES VideoComments(CommentId) ON DELETE CASCADE
);


-- Chỉ mục tối ưu truy vấn
CREATE INDEX IDX_Messages_ConversationID ON Messages(ConversationID);
CREATE INDEX IDX_Conversations_UserID ON Conversations(UserID);
CREATE INDEX IDX_UserDailyUsage_UserID_Date ON UserDailyUsage(UserID, UsageDate);
CREATE INDEX IDX_UserSubscriptions_UserID ON UserSubscriptions(UserID);
CREATE INDEX IDX_Payments_UserID ON Payments(UserID);


CREATE INDEX IX_Users_Username_Email ON [User] (Username, Email);
SELECT * 
FROM sys.indexes
WHERE object_id = OBJECT_ID('User');


/*
select * from likes
select * from [user]
select * from category

ALTER TABLE [OrderDetail]
add  [CategoryName] [nvarchar](120) NOT NULL, ;

alter table [OrderDetail] 
drop column	[CategoryID] [nvarchar](36) NULL,

ALTER TABLE Product
ALTER COLUMN [NewPrice] INT NULL;

ALTER TABLE Product
ALTER COLUMN [ImageUrl] NVARCHAR(max) not NULL;

ALTER TABLE [Order]
ALTER COLUMN [Username] NVARCHAR(255) NOT NULL;

ALTER TABLE posts
add	FOREIGN KEY([CategoryID]) REFERENCES [Category] ([ID])

SELECT 
    CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'Likes' AND CONSTRAINT_TYPE = 'FOREIGN KEY';

INSERT INTO Posts (PostID, UserID, CategoryID, Title, Thumbnail)
VALUES ('post1', '670547e5-621a-460b-8c30-5645f40704c0', 1, 'Sample Post', 'default.jpg');

INSERT INTO  Likes (LikeID, UserID, [EntityID], CreatedAt,[TypeObject])
VALUES ('like1', '670547e5-621a-460b-8c30-5645f40704c0', 'post1', GETDATE(), 1);

INSERT INTO  [Comments] ([CommentID], UserID, [EntityID], CreatedAt,[TypeObject])
VALUES ('c1', '670547e5-621a-460b-8c30-5645f40704c0', 'post1', GETDATE(), 1);

DELETE FROM Posts WHERE PostID = 'post1';

SELECT * 
FROM Posts p
JOIN Category c ON p.CategoryID = c.ID
WHERE c.TypeObject = 2 AND p.Privacy = 0;
DBCC CHECKIDENT ('Category', RESEED, 6);

INSERT INTO Category (Name, TypeObject) VALUES (N'Y tế', 2);