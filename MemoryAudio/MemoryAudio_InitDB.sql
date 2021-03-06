USE [master]
GO
/****** Object:  Database [MemoryAudio]    Script Date: 4/25/18 1:42:30 PM ******/
CREATE DATABASE [MemoryAudio]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MemoryAudio', FILENAME = N'C:\Databases\SQL2012\MemoryAudio.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MemoryAudio_log', FILENAME = N'C:\Databases\SQL2012\MemoryAudio_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [MemoryAudio] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MemoryAudio].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MemoryAudio] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MemoryAudio] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MemoryAudio] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MemoryAudio] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MemoryAudio] SET ARITHABORT OFF 
GO
ALTER DATABASE [MemoryAudio] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MemoryAudio] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MemoryAudio] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MemoryAudio] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MemoryAudio] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MemoryAudio] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MemoryAudio] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MemoryAudio] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MemoryAudio] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MemoryAudio] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MemoryAudio] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MemoryAudio] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MemoryAudio] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MemoryAudio] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MemoryAudio] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MemoryAudio] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MemoryAudio] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MemoryAudio] SET RECOVERY FULL 
GO
ALTER DATABASE [MemoryAudio] SET  MULTI_USER 
GO
ALTER DATABASE [MemoryAudio] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MemoryAudio] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MemoryAudio] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MemoryAudio] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [MemoryAudio] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'MemoryAudio', N'ON'
GO
USE [MemoryAudio]
GO

/****** Object:  Table [dbo].[Categories]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](150) NULL,
	[Description] [nvarchar](1500) NULL,
	[ParentId] [int] NULL,
	[SortIdx] [int] NULL CONSTRAINT [DF_Categories_SortIdx]  DEFAULT ((1000)),
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NULL,
	[ProductName] [nvarchar](150) NULL,
	[Specification] [nvarchar](max) NULL,
	[TotalInStock] [int] NULL CONSTRAINT [DF_Products_TotalInStock]  DEFAULT ((0)),
	[Price] [money] NULL,
	[Discount] [money] NULL,
	[Image1] [nvarchar](150) NULL,
	[Image2] [nvarchar](150) NULL,
	[Image3] [nvarchar](150) NULL,
	[Image4] [nvarchar](150) NULL,
	[Image5] [nvarchar](150) NULL,
	[Image6] [nvarchar](150) NULL,
	[CreationDate] [datetime] NULL CONSTRAINT [DF_Products_CreationDate]  DEFAULT (getdate()),
	[Display] [int] NULL,
	[SortIdx] [int] NULL CONSTRAINT [DF_Products_SortIdx]  DEFAULT ((1000)),
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](64) NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NULL,
	[Password] [nvarchar](512) NULL,
	[Salt] [nvarchar](64) NULL,
	[FullName] [nvarchar](64) NULL,
	[Phone] [nvarchar](128) NULL,
	[Email] [nvarchar](128) NULL,
	[Status] [int] NULL,
	[CreationDate] [datetime] NULL CONSTRAINT [DF_Users_CreationDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[usp_initAdmin]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Camel
-- Create date: April 25, 2018
-- Description:	Initialize administrator account 
--		user: admin
--		pass: 123456aA@
-- =============================================
CREATE PROCEDURE [dbo].[usp_initAdmin] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SET IDENTITY_INSERT [dbo].[Categories] ON 

	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (1, N'Phụ kiện', N'Phụ kiện', NULL, 10)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (2, N'Amplifers', N'Amplifers', NULL, 2)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (3, N'Analog', N'Analog', NULL, 6)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (4, N'Cables', N'Cables', NULL, 1)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (5, N'DA Converters', N'DA Converters', NULL, 5)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (6, N'Home Theaters', N'Home Theaters', NULL, 7)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (7, N'Speakers', N'Speakers', NULL, 3)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (10, N'Tubes', N'Tubes', NULL, 4)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (11, N'Tweaks', N'Tweaks', 1, 4)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (12, N'Receivers', N'Receivers', 6, 1)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (13, N'Solid State', N'Solid State', 2, 2)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (14, N'Tube', N'Tube', 2, 3)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (17, N'Coaxial', N'Coaxial', 4, 2)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (18, N'Interconnect', N'Interconnect', 4, 3)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (19, N'AC Cables', N'AC Cables', 4, 1)
	INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description], [ParentId], [SortIdx]) VALUES (20, N'Speaker Cables', N'Speaker Cables', 4, 4)
	SET IDENTITY_INSERT [dbo].[Categories] OFF
	SET IDENTITY_INSERT [dbo].[Roles] ON 

	INSERT [dbo].[Roles] ([RoleId], [RoleName], [Description]) VALUES (1, N'Administrators', N'Administrators')
	SET IDENTITY_INSERT [dbo].[Roles] OFF
	INSERT [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (1, 1)
	SET IDENTITY_INSERT [dbo].[Users] ON 

	INSERT [dbo].[Users] ([UserId], [Username], [Password], [Salt], [FullName], [Phone], [Email], [Status], [CreationDate]) VALUES (1, N'admin', N'FdkPRCP74Udv0f9oNedikCo0/6ZpeTKKCATxf104ttB8B0ITGJ8WqcpPY+g5L45tr+570Fi+c6UuowekHl7el8O+nzSyanRLpNvy0rZaeFGLb3Rg8waMlMMis8LZzez1l0G/crbZgaKbZymkzIuj7Z67TpUYRLREdRBYWHlo4o8DcDYx9sSyZZbDoDTuotwgiG1V1DAHGOmC6kvAYkT6S8Etd+bep5xkz0qqF5A/oV5u7I43MTmZj0J+P+L4cawaefnPhiBQgBUu6bKskDIne1Be1XViqU7Ldf2dV9JqVypeX3sB8aN33EljatA5jQehE2FK/U1Det2xsgg/sNoE4Q==', N'd4yxSs7vFgGhDxfyEVdTHe7y85k4ejPuZU5jiNfXfkA=', N'NGUYEN PHUOC LOC', N'909841682', N'locnp.saigon@gmail.com', 2, CAST(N'2018-04-20 16:06:42.820' AS DateTime))
	SET IDENTITY_INSERT [dbo].[Users] OFF


END

GO
/****** Object:  StoredProcedure [dbo].[usp_initSampleData]    Script Date: 4/25/18 1:42:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Camel
-- Create date: April 13, 2018
-- Description:	Initialize sample data
-- =============================================
CREATE PROCEDURE [dbo].[usp_initSampleData]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	TRUNCATE TABLE [dbo].[Products]

    -- Insert statements for procedure here
	DECLARE @count int
	DECLARE @CategoryId int
	DECLARE @ProductName nvarchar(150)
	DECLARE @CategoryName nvarchar(150)
	DECLARE @Specification nvarchar(150)
	DECLARE @TotalInStock int
	DECLARE @Price money
	DECLARE @Discount money
	DECLARE @Image1 nvarchar(150)
	DECLARE @Image2 nvarchar(150)
	DECLARE @Image3 nvarchar(150)
	DECLARE @Image4 nvarchar(150)
	DECLARE @Image5 nvarchar(150)
	DECLARE @Image6 nvarchar(150)
	DECLARE @Display int
	
	DECLARE @RandNum int

	SET @count  = 0

	WHILE @count < 1000
	BEGIN
	
		-- SELECT RANDOM CATEGORY
		SELECT TOP 1 @CategoryId = CategoryId, @CategoryName = CategoryName 
		FROM Categories (NOLOCK)
		WHERE CategoryId > 2
		ORDER BY NEWID()

		SET @ProductName = @CategoryName + N' ' + CAST(@count as nvarchar(5))
		SET @Specification = @CategoryName + N' specification ' +  CAST(@count as nvarchar(5))
		SET @TotalInStock = ABS(CHECKSUM(NEWID())) % 50
		SET @Price = ((ABS(CHECKSUM(NEWID())) % 14) + 1) * 35000
		SET @Discount = (ABS(CHECKSUM(NEWID())) % 14) * 20000 
		
		-- Randomize images
		SET @RandNum = ((ABS(CHECKSUM(NEWID())) % 6) + 1)
		EXEC @Image1 = [dbo].[ufn_getRandomImage] @RandNum
		
		SET @RandNum = ((ABS(CHECKSUM(NEWID())) % 6) + 1)	
		EXEC @Image2 = [dbo].[ufn_getRandomImage] @RandNum

		SET @RandNum = ((ABS(CHECKSUM(NEWID())) % 6) + 1)
		EXEC @Image3 = [dbo].[ufn_getRandomImage] @RandNum
		
		SET @RandNum = ((ABS(CHECKSUM(NEWID())) % 6) + 1)
		EXEC @Image4 = [dbo].[ufn_getRandomImage] @RandNum

		SET @RandNum = ((ABS(CHECKSUM(NEWID())) % 6) + 1)
		EXEC @Image5 = [dbo].[ufn_getRandomImage] @RandNum

		SET @RandNum = ((ABS(CHECKSUM(NEWID())) % 6) + 1)
		EXEC @Image6 = [dbo].[ufn_getRandomImage] @RandNum

		SET @Display = ABS(CHECKSUM(NEWID())) % 4

		INSERT INTO [dbo].[Products]
			   ([CategoryId]
			   ,[ProductName]
			   ,[Specification]
			   ,[TotalInStock]
			   ,[Price]
			   ,[Discount]
			   ,[Image1]
			   ,[Image2]
			   ,[Image3]
			   ,[Image4]
			   ,[Image5]
			   ,[Image6]
			   ,[Display])
		 VALUES
			   (@CategoryId
			   ,@ProductName
			   ,@Specification
			   ,@TotalInStock
			   ,@Price
			   ,@Discount
			   ,@Image1
			   ,@Image2
			   ,@Image3
			   ,@Image4
			   ,@Image5
			   ,@Image6
			   ,@Display)


		SET @count = @count + 1
	END



END

GO
USE [master]
GO
ALTER DATABASE [MemoryAudio] SET  READ_WRITE 
GO
