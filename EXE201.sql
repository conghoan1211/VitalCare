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
    [NewPrice] [int] NOT NULL,
    [ImageUrl] [nvarchar](500) NULL,
    [ProductUrl] [nvarchar](500) NOT NULL,
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




select * from [comments]

alter table likes 
drop column	[PostID]

alter table likes 
add	[EntityID] [varchar](36) NOT NULL


ALTER TABLE posts
add	FOREIGN KEY([CategoryID]) REFERENCES [Category] ([ID])

SELECT 
    CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'Likes' AND CONSTRAINT_TYPE = 'FOREIGN KEY';

ALTER TABLE Likes DROP CONSTRAINT FK__Likes__PostID__3D5E1FD2;

ALTER TABLE Likes
ADD  FOREIGN KEY (EntityID)
REFERENCES Posts (PostID) ON DELETE CASCADE;

*/
