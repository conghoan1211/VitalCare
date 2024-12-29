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
)

CREATE TABLE [Posts](
	[PostID] [varchar](36) primary key NOT NULL ,
	[UserID] [varchar](36) NOT NULL,
	[Content] [nvarchar](500) NULL,
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
	FOREIGN KEY([UserID]) REFERENCES [User] ([UserID])
)

CREATE TABLE Category (
    [ID] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(255),
	[TypeObject] INT NOT NULL,
	[IsActive] BIT NULL,
	[IsDeleted] BIT NULL,
);

CREATE TABLE Product (
	[ProductID] VARCHAR(36) PRIMARY KEY,
    [Title] NVARCHAR(255) NOT NULL,
    [Price] int NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [ProductUrl] NVARCHAR(500) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [CategoryID] INT NOT NULL, 
	[Sold] INT NULL,
    [Stock] INT NULL,        
    [Status] INT NULL,      -- yeu thich/ het hang/ khuyen mai/ sale...	 	
	[Rating] FLOAT NULL,
	[IsDeleted] BIT NULL,
	[IsActive] BIT NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
 
	FOREIGN KEY ([CategoryID])	REFERENCES Category(ID),
);


CREATE INDEX IX_Users_Username_Email ON [User] (Username, Email);
SELECT * 
FROM sys.indexes
WHERE object_id = OBJECT_ID('User');



-- add user  LastLogin, 

select * from [User]