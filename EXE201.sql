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
	[CommentID] [varchar](36) NOT NULL,
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
	[LikeID] [varchar](36) NOT NULL,
	[UserID] [varchar](36) NOT NULL,
	[EntityID] [varchar](36) NOT NULL,
	[TypeObject] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,

    FOREIGN KEY([EntityID]) REFERENCES [dbo].[Posts] ([PostID])  ON DELETE CASCADE,
    FOREIGN KEY([UserID] ) REFERENCES [dbo].[User] ([UserID])
)


CREATE INDEX IX_Users_Username_Email ON [User] (Username, Email);
SELECT * 
FROM sys.indexes
WHERE object_id = OBJECT_ID('User');


/*

select * from likes
select * from [user]

alter table [user]
add [Address] nvarchar(255) null

alter table Product 
add	[UpdateUser] [nvarchar](36) NULL,

ALTER TABLE Product
ALTER COLUMN [NewPrice] INT NULL;

ALTER TABLE Product
ALTER COLUMN [ImageUrl] NVARCHAR(max) not NULL;

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
