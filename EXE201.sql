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
	
	FOREIGN KEY (EntityID) REFERENCES Videos(VideoID) ON DELETE CASCADE,
    FOREIGN KEY([EntityID]) REFERENCES [dbo].[Posts] ([PostID]) ON DELETE CASCADE,
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
    Duration [nvarchar](100) NULL, -- Thời lượng video (giây)
    CategoryId [int] NULL, -- Danh mục video
	[Views] [int] NULL,
	[Likes] [int] NULL,
	[IsActive] [bit] NULL,
    CreatedAt [datetime] DEFAULT GETDATE(),
    UpdatedAt [datetime],
	[CreateUser] [nvarchar](36) NULL,
	[UpdateUser] [nvarchar](36) NULL,
	FOREIGN KEY ([CategoryID]) REFERENCES Category(ID)
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
    FOREIGN KEY (ParentCommentId) REFERENCES VideoComments(CommentId) ON DELETE NO ACTION --- cần xóa các bl reply trước khi xóa bl gốc.
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

SELECT 
    i.name AS IndexName, 
    t.name AS TableName, 
    c.name AS ColumnName, 
    i.type_desc AS IndexType
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.tables t ON i.object_id = t.object_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id

/*
select * from likes
select * from [user]
select * from category

ALTER TABLE [videos]
add  [IsActive] [bit] NULL ;

alter table [OrderDetail] 
drop column	[CategoryID] [nvarchar](36) NULL,

ALTER TABLE videos
ALTER COLUMN [CategoryId] INT NULL;

ALTER TABLE Product
ALTER COLUMN [ImageUrl] NVARCHAR(max) not NULL;

ALTER TABLE [Videos]
ALTER COLUMN [Duration] NVARCHAR(100) NULL;

ALTER TABLE videos
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

*/

INSERT INTO [User] (
    [UserID], [Username], [Phone], [Email], [Password], [RoleID],
    [GoogleID], [Sex], [Dob], [Bio], [Address],
    [ProvinceID], [DistrictID], [DistrictName], [ProvinceName],
    [IsDisable], [IsActive], [IsVerified], [CreateAt], [UpdateAt],
    [CreateUser], [UpdateUser], [Status], [LastLogin], [LastLoginIP],
    [RefreshToken], [ExpiryDateToken]
)
VALUES
-- User 1
(NEWID(), N'Nguyễn Văn An', '0912345678', 'nguyenvanan@gmail.com', '123456@A', 1,
 NULL, 1, '1995-05-20', N'Yêu thể thao', N'123 Nguyễn Trãi, Thanh Xuân',
 '01', '001', N'Thanh Xuân', N'Hà Nội',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 1, GETDATE(), '192.168.1.10',
 NULL, NULL),

-- User 2
(NEWID(), N'Trần Thị Bích', '0987654321', 'tranthibich@yahoo.com', 'abc@123', 2,
 NULL, 0, '1998-08-15', N'Mê đọc sách', N'56 Lê Duẩn, Quận 1',
 '79', '760', N'Quận 1', N'Hồ Chí Minh',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 0, GETDATE(), '192.168.1.11',
 NULL, NULL),

-- User 3
(NEWID(), N'Lê Quốc Cường', '0909123456', 'lecuong@gmail.com', 'cuong123', 1,
 NULL, 1, '1990-12-30', N'Đam mê công nghệ', N'12 Phạm Văn Đồng, Thủ Đức',
 '79', '769', N'Thủ Đức', N'Hồ Chí Minh',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 1, GETDATE(), '192.168.1.12',
 NULL, NULL),

-- User 4
(NEWID(), N'Phạm Thị Dung', '0933456789', 'pham.dung@gmail.com', 'dung@456', 2,
 NULL, 0, '1993-03-10', N'Thích du lịch', N'99 Bạch Mai, Hai Bà Trưng',
 '01', '002', N'Hai Bà Trưng', N'Hà Nội',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 1, GETDATE(), '192.168.1.13',
 NULL, NULL),

-- User 5
(NEWID(), N'Hoàng Minh Tú', '0923456789', 'hoangtu@gmail.com', 'tu@789', 1,
 NULL, 1, '2000-07-22', N'Sống tích cực', N'15 Trần Hưng Đạo, Ninh Kiều',
 '92', '916', N'Ninh Kiều', N'Cần Thơ',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 0, GETDATE(), '192.168.1.14',
 NULL, NULL);


 INSERT INTO [User] (
    [UserID], [Username], [Phone], [Email], [Password], [RoleID],
    [GoogleID], [Sex], [Dob], [Bio], [Address],
    [ProvinceID], [DistrictID], [DistrictName], [ProvinceName],
    [IsDisable], [IsActive], [IsVerified], [CreateAt], [UpdateAt],
    [CreateUser], [UpdateUser], [Status], [LastLogin], [LastLoginIP],
    [RefreshToken], [ExpiryDateToken]
)
VALUES
-- User 6
(NEWID(), N'Nguyễn Thị Hồng', '0911223344', 'hong.nguyen@gmail.com', 'hong123', 1,
 NULL, 0, '1996-09-18', N'Thích nấu ăn', N'34 Trần Phú, Ba Đình',
 '01', '003', N'Ba Đình', N'Hà Nội',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 1, GETDATE(), '192.168.1.15',
 NULL, NULL),

-- User 7
(NEWID(), N'Đặng Văn Bình', '0977112233', 'binh.dang@gmail.com', 'binh456', 1,
 NULL, 1, '1988-11-05', N'Tham gia thiện nguyện', N'21 Nguyễn Văn Cừ, Long Biên',
 '01', '004', N'Long Biên', N'Hà Nội',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 0, GETDATE(), '192.168.1.16',
 NULL, NULL),

-- User 8
(NEWID(), N'Trương Thị Mai', '0966334455', 'truong.mai@gmail.com', 'mai789', 2,
 NULL, 0, '1999-04-02', N'Mê làm đẹp', N'88 Cách Mạng Tháng 8, Quận 3',
 '79', '764', N'Quận 3', N'Hồ Chí Minh',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 1, GETDATE(), '192.168.1.17',
 NULL, NULL),

-- User 9
(NEWID(), N'Phan Văn Lộc', '0909777888', 'phan.loc@gmail.com', 'locabc123', 1,
 NULL, 1, '1992-01-30', N'Chơi guitar', N'12 Nguyễn Văn Linh, Hải Châu',
 '48', '494', N'Hải Châu', N'Đà Nẵng',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 1, GETDATE(), '192.168.1.18',
 NULL, NULL),

-- User 10
(NEWID(), N'Võ Thị Thu Hà', '0933555777', 'vo.ha@gmail.com', 'ha123@abc', 2,
 NULL, 0, '1997-10-10', N'Thích nuôi thú cưng', N'100 Nguyễn Hữu Thọ, TP. Vinh',
 '40', '401', N'TP. Vinh', N'Nghệ An',
 0, 1, 1, GETDATE(), GETDATE(),
 NULL, NULL, 0, GETDATE(), '192.168.1.19',
 NULL, NULL);

 INSERT INTO [User] (
    [UserID], [Username], [Phone], [Email], [Password], [RoleID],
    [GoogleID], [Sex], [Dob], [Bio], [Address],
    [ProvinceID], [DistrictID], [DistrictName], [ProvinceName],
    [IsDisable], [IsActive], [IsVerified], [CreateAt], [UpdateAt],
    [CreateUser], [UpdateUser], [Status], [LastLogin], [LastLoginIP],
    [RefreshToken], [ExpiryDateToken]
)
VALUES
(NEWID(), N'Nguyễn Văn Nam', '0912345678', 'nam.nguyen@gmail.com', 'password123', 1,
NULL, 1, '1993-04-12', N'Yêu thích công nghệ', N'Số 12 Đường Láng, Đống Đa',
'01', '003', N'Đống Đa', N'Hà Nội',
0, 1, 1, GETDATE(), GETDATE(),
NULL, NULL, 1, GETDATE(), '192.168.1.10',
NULL, NULL),

(NEWID(), N'Phạm Thị Hương', '0987654321', 'huongpham1995x@gmail.com', 'abc123xyz', 2,
NULL, 0, '1995-07-23', N'Thích đọc sách', N'Số 10, Lê Duẩn, Hoàn Kiếm',
'01', '002', N'Hoàn Kiếm', N'Hà Nội',
0, 1, 1, GETDATE(), GETDATE(),
NULL, NULL, 1, GETDATE(), '192.168.1.11',
NULL, NULL),

(NEWID(), N'Lê Hoàng Anh', '0978123456', 'hoanganhle19900915@gmail.com', 'pass456', 1,
NULL, 1, '1990-09-15', N'Người năng động', N'23 Trần Phú, Ba Đình',
'01', '001', N'Ba Đình', N'Hà Nội',
0, 1, 1, GETDATE(), GETDATE(),
NULL, NULL, 1, GETDATE(), '192.168.1.12',
NULL, NULL),

(NEWID(), N'Trần Thị Mai', '0962345678', 'Qmaitran19920202@hotmail.com', 'mai2024', 2,
NULL, 0, '1992-02-02', N'Yêu thiên nhiên', N'5 Nguyễn Chí Thanh, Cầu Giấy',
'01', '005', N'Cầu Giấy', N'Hà Nội',
0, 1, 1, GETDATE(), GETDATE(),
NULL, NULL, 1, GETDATE(), '192.168.1.13',
NULL, NULL),

(NEWID(), N'Đỗ Minh Tuấn', '0945678910', 'tuando19881125oo@gmail.com', 'tuansecure', 1,
NULL, 1, '1988-11-25', N'Yêu bóng đá', N'78 Trần Duy Hưng, Thanh Xuân',
'01', '006', N'Thanh Xuân', N'Hà Nội',
0, 1, 1, GETDATE(), GETDATE(),
NULL, NULL, 1, GETDATE(), '192.168.1.14',
NULL, NULL);

INSERT INTO [User] ([UserID], [Username], [Phone], [Email], [Password], [RoleID], [GoogleID], [Sex], [Dob], [Bio], [Address], [ProvinceID], [DistrictID], [DistrictName], [ProvinceName], [IsDisable], [IsActive], [IsVerified], [CreateAt], [UpdateAt], [CreateUser], [UpdateUser], [Status], [LastLogin], [LastLoginIP], [RefreshToken], [ExpiryDateToken]) VALUES
-- 1
(NEWID(), N'Nguyễn Văn Nam', '0987654321', 'nam.nguyen@gmail.com', 'abc123', 1, NULL, 1, '1998-06-15', N'Tôi yêu cuộc sống', N'12 Nguyễn Trãi', '01', '007', N'Hoàng Mai', N'Hà Nội', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.10', NULL, NULL),
-- 2
(NEWID(), N'Lê Thị Hằng', '0912345678', 'hang.le@gmail.com', 'abc123', 2, NULL, 0, '1995-04-12', N'Thích đọc sách', N'56 Trần Duy Hưng', '01', '008', N'Hai Bà Trưng', N'Hà Nội', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.11', NULL, NULL),
-- 3
(NEWID(), N'Trần Quang Huy', '0932123456', 'huy.tran@gmail.com', 'abc123', 1, NULL, 1, '1990-12-30', N'Yêu công nghệ', N'89 Lê Lợi', '01', '009', N'Long Biên', N'Hà Nội', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.12', NULL, NULL),
-- 4
(NEWID(), N'Phạm Thị Lan', '0961234567', 'lan.pham@gmail.com', 'abc123', 2, NULL, 0, '1993-08-05', N'Đam mê nấu ăn', N'23 Phan Chu Trinh', '02', '022', N'Ngô Quyền', N'Hải Phòng', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.13', NULL, NULL),
-- 5
(NEWID(), N'Đỗ Văn Minh', '0977654321', 'minh.do@gmail.com', 'abc123', 1, NULL, 1, '1992-03-25', N'Thích thể thao', N'101 Lý Thường Kiệt', '03', '031', N'Hạ Long', N'Quảng Ninh', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.14', NULL, NULL),
-- 6
(NEWID(), N'Bùi Thị Mai', '0901234567', 'mai.bui@gmail.com', 'abc123', 2, NULL, 0, '1996-11-20', N'Tôi thích du lịch', N'14 Nguyễn Du', '04', '041', N'TP Bắc Ninh', N'Bắc Ninh', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.15', NULL, NULL),
-- 7
(NEWID(), N'Hoàng Anh Tuấn', '0943215678', 'tuan.hoang@gmail.com', 'abc123', 1, NULL, 1, '1991-01-10', N'Tôi là lập trình viên', N'88 Quang Trung', '05', '051', N'TP Thái Nguyên', N'Thái Nguyên', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.16', NULL, NULL),
-- 8
(NEWID(), N'Vũ Thị Hương', '0923456789', 'huong.vu@gmail.com', 'abc123', 2, NULL, 0, '1994-07-07', N'Thích hát và vẽ', N'37 Hùng Vương', '07', '071', N'TP Yên Bái', N'Yên Bái', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.17', NULL, NULL),
-- 9
(NEWID(), N'Ngô Văn Sơn', '0911222333', 'son.ngo@gmail.com', 'abc123', 1, NULL, 1, '1997-05-18', N'Yêu thiên nhiên', N'55 Lạc Long Quân', '09', '091', N'Việt Trì', N'Phú Thọ', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.18', NULL, NULL),
-- 10
(NEWID(), N'Tạ Thị Nhung', '0988112233', 'nhung.ta@gmail.com', 'abc123', 2, NULL, 0, '1999-10-01', N'Cuộc sống muôn màu', N'22 Thanh Niên', '13', '131', N'TP Ninh Bình', N'Ninh Bình', 0, 1, 1, GETDATE(), GETDATE(), NULL, NULL, 1, GETDATE(), '192.168.1.19', NULL, NULL);
