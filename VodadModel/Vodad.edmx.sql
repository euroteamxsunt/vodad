
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/02/2013 17:44:28
-- Generated from EDMX file: D:\Distr\Vodad\Vodad\VodadModel\Vodad.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Vodad];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AlertMessage_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AlertMessage] DROP CONSTRAINT [FK_AlertMessage_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Ban_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ban] DROP CONSTRAINT [FK_Ban_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_Ban_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ban] DROP CONSTRAINT [FK_Ban_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Ban_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ban] DROP CONSTRAINT [FK_Ban_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_Ban_User2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ban] DROP CONSTRAINT [FK_Ban_User2];
GO
IF OBJECT_ID(N'[dbo].[FK_BlackList_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BlackList] DROP CONSTRAINT [FK_BlackList_User];
GO
IF OBJECT_ID(N'[dbo].[FK_BlackList_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BlackList] DROP CONSTRAINT [FK_BlackList_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_Certificates_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Certificates] DROP CONSTRAINT [FK_Certificates_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Cheaters_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cheaters] DROP CONSTRAINT [FK_Cheaters_User];
GO
IF OBJECT_ID(N'[dbo].[FK_GeolocationPlatformPercentage_Geolocation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeolocationPlatformPercentage] DROP CONSTRAINT [FK_GeolocationPlatformPercentage_Geolocation];
GO
IF OBJECT_ID(N'[dbo].[FK_GeolocationPlatformPercentagePerformerPlatform]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeolocationPlatformPercentage] DROP CONSTRAINT [FK_GeolocationPlatformPercentagePerformerPlatform];
GO
IF OBJECT_ID(N'[dbo].[FK_Image_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Image] DROP CONSTRAINT [FK_Image_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Messages_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_Messages_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Messages_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_Messages_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_MoneyTransfers_Merchants]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MoneyTransfers] DROP CONSTRAINT [FK_MoneyTransfers_Merchants];
GO
IF OBJECT_ID(N'[dbo].[FK_MoneyTransfers_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MoneyTransfers] DROP CONSTRAINT [FK_MoneyTransfers_User];
GO
IF OBJECT_ID(N'[dbo].[FK_News_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[News] DROP CONSTRAINT [FK_News_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Order_Regions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_Order_Regions];
GO
IF OBJECT_ID(N'[dbo].[FK_Order_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_Order_User];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderPerformed_OrderImages]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderPerformed] DROP CONSTRAINT [FK_OrderPerformed_OrderImages];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderPerformed_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderPerformed] DROP CONSTRAINT [FK_OrderPerformed_User];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderThemes_Order]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderThemes] DROP CONSTRAINT [FK_OrderThemes_Order];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderThemes_Theme]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderThemes] DROP CONSTRAINT [FK_OrderThemes_Theme];
GO
IF OBJECT_ID(N'[dbo].[FK_PerformerPlatform_Themes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PerformerPlatform] DROP CONSTRAINT [FK_PerformerPlatform_Themes];
GO
IF OBJECT_ID(N'[dbo].[FK_PerformerPlatform_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PerformerPlatform] DROP CONSTRAINT [FK_PerformerPlatform_User];
GO
IF OBJECT_ID(N'[dbo].[FK_PerformerStatistics_GeolocationPlatformPercentage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PerformerStatistics] DROP CONSTRAINT [FK_PerformerStatistics_GeolocationPlatformPercentage];
GO
IF OBJECT_ID(N'[dbo].[FK_Ticket_Image]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tickets] DROP CONSTRAINT [FK_Ticket_Image];
GO
IF OBJECT_ID(N'[dbo].[FK_Ticket_Ticket]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tickets] DROP CONSTRAINT [FK_Ticket_Ticket];
GO
IF OBJECT_ID(N'[dbo].[FK_Ticket_TicketThemes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tickets] DROP CONSTRAINT [FK_Ticket_TicketThemes];
GO
IF OBJECT_ID(N'[dbo].[FK_Ticket_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tickets] DROP CONSTRAINT [FK_Ticket_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Ticket_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tickets] DROP CONSTRAINT [FK_Ticket_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_Ticket_User2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tickets] DROP CONSTRAINT [FK_Ticket_User2];
GO
IF OBJECT_ID(N'[dbo].[FK_Transactions_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_Transactions_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Transactions_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_Transactions_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_User_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_User_Timezone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Timezone];
GO
IF OBJECT_ID(N'[dbo].[FK_User_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMerchants_Merchants]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMerchants] DROP CONSTRAINT [FK_UserMerchants_Merchants];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMerchants_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMerchants] DROP CONSTRAINT [FK_UserMerchants_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Video_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Video] DROP CONSTRAINT [FK_Video_User];
GO
IF OBJECT_ID(N'[dbo].[FK_Wallet_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Wallet] DROP CONSTRAINT [FK_Wallet_User];
GO
IF OBJECT_ID(N'[dbo].[FK_WhiteList_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WhiteList] DROP CONSTRAINT [FK_WhiteList_User];
GO
IF OBJECT_ID(N'[dbo].[FK_WhiteList_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WhiteList] DROP CONSTRAINT [FK_WhiteList_User1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[__RefactorLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[__RefactorLog];
GO
IF OBJECT_ID(N'[dbo].[AlertMessage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AlertMessage];
GO
IF OBJECT_ID(N'[dbo].[Ban]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ban];
GO
IF OBJECT_ID(N'[dbo].[BlackList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BlackList];
GO
IF OBJECT_ID(N'[dbo].[Certificates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Certificates];
GO
IF OBJECT_ID(N'[dbo].[Cheaters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cheaters];
GO
IF OBJECT_ID(N'[dbo].[Geolocation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Geolocation];
GO
IF OBJECT_ID(N'[dbo].[GeolocationPlatformPercentage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeolocationPlatformPercentage];
GO
IF OBJECT_ID(N'[dbo].[Image]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Image];
GO
IF OBJECT_ID(N'[dbo].[Merchants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Merchants];
GO
IF OBJECT_ID(N'[dbo].[Messages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Messages];
GO
IF OBJECT_ID(N'[dbo].[MoneyTransfers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MoneyTransfers];
GO
IF OBJECT_ID(N'[dbo].[News]', 'U') IS NOT NULL
    DROP TABLE [dbo].[News];
GO
IF OBJECT_ID(N'[dbo].[Order]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Order];
GO
IF OBJECT_ID(N'[dbo].[OrderContent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderContent];
GO
IF OBJECT_ID(N'[dbo].[OrderPerformed]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPerformed];
GO
IF OBJECT_ID(N'[dbo].[OrderThemes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderThemes];
GO
IF OBJECT_ID(N'[dbo].[PerformerPlatform]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PerformerPlatform];
GO
IF OBJECT_ID(N'[dbo].[PerformerStatistics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PerformerStatistics];
GO
IF OBJECT_ID(N'[dbo].[Regions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Regions];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[Themes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Themes];
GO
IF OBJECT_ID(N'[dbo].[Tickets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tickets];
GO
IF OBJECT_ID(N'[dbo].[TicketThemes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TicketThemes];
GO
IF OBJECT_ID(N'[dbo].[Timezone]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Timezone];
GO
IF OBJECT_ID(N'[dbo].[Transactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Transactions];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO
IF OBJECT_ID(N'[dbo].[UserMerchants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserMerchants];
GO
IF OBJECT_ID(N'[dbo].[Video]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Video];
GO
IF OBJECT_ID(N'[dbo].[Wallet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Wallet];
GO
IF OBJECT_ID(N'[dbo].[WhiteList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WhiteList];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'C__RefactorLog'
CREATE TABLE [dbo].[C__RefactorLog] (
    [OperationKey] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AlertMessage'
CREATE TABLE [dbo].[AlertMessage] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreatorId] bigint  NULL,
    [Text] nvarchar(500)  NULL,
    [CreationDate] datetime  NULL
);
GO

-- Creating table 'Ban'
CREATE TABLE [dbo].[Ban] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [RoleId] bigint  NULL,
    [BanDateTime] datetime  NULL,
    [UnbanDateTime] datetime  NULL,
    [BanReason] nvarchar(4000)  NULL,
    [UnbanReason] nvarchar(4000)  NULL,
    [CanBeUnbanned] bit  NULL,
    [AdminBanId] bigint  NULL,
    [AdminUnbanId] bigint  NULL
);
GO

-- Creating table 'BlackList'
CREATE TABLE [dbo].[BlackList] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OwnerId] bigint  NULL,
    [UserId] bigint  NULL
);
GO

-- Creating table 'Certificates'
CREATE TABLE [dbo].[Certificates] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [Code] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL
);
GO

-- Creating table 'Cheaters'
CREATE TABLE [dbo].[Cheaters] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL
);
GO

-- Creating table 'Geolocation'
CREATE TABLE [dbo].[Geolocation] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CountryName] nvarchar(50)  NULL,
    [ISO2] nvarchar(max)  NULL,
    [LongCountryName] nvarchar(max)  NULL,
    [ISO3] nvarchar(max)  NULL,
    [NumCode] nvarchar(max)  NULL,
    [UNMemberState] nvarchar(max)  NULL,
    [CallingCode] nvarchar(max)  NULL,
    [CCTLD] nvarchar(max)  NULL
);
GO

-- Creating table 'GeolocationPlatformPercentage'
CREATE TABLE [dbo].[GeolocationPlatformPercentage] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [GeolocationId] bigint  NULL,
    [Percentage] bigint  NULL,
    [PerformerPlatformId] bigint  NOT NULL
);
GO

-- Creating table 'Image'
CREATE TABLE [dbo].[Image] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NOT NULL,
    [ImageName] nvarchar(255)  NOT NULL,
    [ImageData] varbinary(max)  NOT NULL,
    [ImageSize] bigint  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [ImageWidth] int  NULL,
    [ImageHeight] int  NULL,
    [Extension] nvarchar(40)  NULL
);
GO

-- Creating table 'Merchants'
CREATE TABLE [dbo].[Merchants] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [MerchantName] nvarchar(50)  NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [FromUserId] bigint  NULL,
    [ToUserId] bigint  NULL,
    [CreationDate] datetime  NULL,
    [MessageText] nvarchar(4000)  NULL,
    [IsRead] bit  NULL,
    [MessageTitle] nvarchar(50)  NULL,
    [IsDeletedForAuthor] bit  NULL,
    [IsDeletedForReciever] bit  NULL
);
GO

-- Creating table 'MoneyTransfers'
CREATE TABLE [dbo].[MoneyTransfers] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [DateTime] datetime  NULL,
    [Action] nvarchar(50)  NULL,
    [Amount] decimal(25,13)  NULL,
    [MerchantId] bigint  NULL,
    [AccountMerchantSystem] nvarchar(50)  NULL,
    [TransactionId] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [StatusDescription] nvarchar(4000)  NULL
);
GO

-- Creating table 'News'
CREATE TABLE [dbo].[News] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreatorId] bigint  NULL,
    [Title] nvarchar(100)  NULL,
    [Text] nvarchar(4000)  NULL,
    [Text1] nvarchar(4000)  NULL,
    [Text2] nvarchar(4000)  NULL,
    [CreationDate] datetime  NULL
);
GO

-- Creating table 'Order'
CREATE TABLE [dbo].[Order] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [Comment] nvarchar(4000)  NULL,
    [CreationDate] datetime  NULL,
    [ExpireDate] datetime  NULL,
    [Status] nvarchar(50)  NULL,
    [RegionId] bigint  NULL,
    [Name] nvarchar(100)  NULL
);
GO

-- Creating table 'OrderContent'
CREATE TABLE [dbo].[OrderContent] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [IdOrder] bigint  NOT NULL,
    [IdContent] bigint  NOT NULL,
    [Status] nvarchar(50)  NULL,
    [ContentType] nvarchar(50)  NULL
);
GO

-- Creating table 'OrderPerformed'
CREATE TABLE [dbo].[OrderPerformed] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Status] nvarchar(50)  NULL,
    [MoneyPaid] decimal(25,13)  NULL,
    [OrderContentId] bigint  NULL,
    [AuthorId] bigint  NOT NULL,
    [IsLiked] bit  NULL
);
GO

-- Creating table 'OrderThemes'
CREATE TABLE [dbo].[OrderThemes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OrderId] bigint  NULL,
    [ThemeId] bigint  NULL
);
GO

-- Creating table 'PerformerPlatform'
CREATE TABLE [dbo].[PerformerPlatform] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [PerformerId] bigint  NULL,
    [Login] nvarchar(50)  NULL,
    [Password] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [ThemeId] bigint  NULL,
    [Verified] nvarchar(50)  NULL,
    [LastOnlineDateTime] datetime  NULL,
    [Link] nvarchar(200)  NULL,
    [Date] datetime  NULL
);
GO

-- Creating table 'PerformerStatistics'
CREATE TABLE [dbo].[PerformerStatistics] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [PerformerPlatformId] bigint  NULL,
    [AverageViewerCountPerHour] bigint  NULL,
    [MaxViewersCount] bigint  NULL,
    [TotalUniqueViews] bigint  NULL,
    [TotalFollowers] bigint  NULL,
    [TotalViews] decimal(18,0)  NULL,
    [Likes] bigint  NULL,
    [CompletedOrders] bigint  NULL,
    [TotalOrders] bigint  NULL,
    [AverageComplitionSpeed] decimal(18,0)  NULL,
    [DateModified] datetime  NOT NULL,
    [Status] nvarchar(max)  NULL,
    [UniqueViewersForMonth] bigint  NULL
);
GO

-- Creating table 'Regions'
CREATE TABLE [dbo].[Regions] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RegionName] nvarchar(50)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(50)  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'Themes'
CREATE TABLE [dbo].[Themes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL
);
GO

-- Creating table 'Tickets'
CREATE TABLE [dbo].[Tickets] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(500)  NULL,
    [ThemeId] bigint  NULL,
    [Text1] nvarchar(4000)  NULL,
    [Text2] nvarchar(4000)  NULL,
    [Text3] nvarchar(4000)  NULL,
    [ImageId] bigint  NULL,
    [CreationDate] datetime  NULL,
    [AnswerDate] datetime  NULL,
    [CloseDate] datetime  NULL,
    [ParentTicketId] bigint  NULL,
    [AdminAnswer] nvarchar(4000)  NULL,
    [AdminCloseComment] nvarchar(4000)  NULL,
    [CreatorId] bigint  NULL,
    [AnswerAdminId] bigint  NULL,
    [CloseAdminId] bigint  NULL,
    [Status] nvarchar(50)  NULL
);
GO

-- Creating table 'TicketThemes'
CREATE TABLE [dbo].[TicketThemes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ThemeName] nvarchar(50)  NULL
);
GO

-- Creating table 'Timezone'
CREATE TABLE [dbo].[Timezone] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NULL
);
GO

-- Creating table 'Transactions'
CREATE TABLE [dbo].[Transactions] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [FromUserId] bigint  NOT NULL,
    [Amount] decimal(24,13)  NOT NULL,
    [ToUserId] bigint  NOT NULL,
    [DateTime] datetime  NOT NULL
);
GO

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Login] nvarchar(50)  NULL,
    [Email] nvarchar(100)  NULL,
    [Password] nvarchar(128)  NULL,
    [RoleId] bigint  NULL,
    [RegistrationDate] datetime  NULL,
    [TimeZoneId] bigint  NULL,
    [ReferrerId] bigint  NULL,
    [Status] nvarchar(50)  NULL,
    [PasswordSalt] nvarchar(128)  NOT NULL,
    [Comments] nvarchar(4000)  NULL,
    [LastModifiedDate] datetime  NULL,
    [LastLoginDate] datetime  NOT NULL,
    [LastLoginIp] nvarchar(40)  NULL,
    [IsActivated] bit  NOT NULL,
    [IsLockedOut] bit  NOT NULL,
    [LastLockedOutDate] datetime  NOT NULL,
    [LastLockedOutReason] nvarchar(256)  NULL,
    [NewPasswordKey] nvarchar(128)  NULL,
    [NewEmail] nvarchar(100)  NULL,
    [NewEmailKey] nvarchar(128)  NULL,
    [NewEmailRequested] datetime  NULL,
    [Rating] bigint  NULL,
    [Karma] bigint  NULL,
    [LastOnlineDateTime] datetime  NULL,
    [LastEmailSendDateTime] datetime  NULL,
    [AccountAttachedIP] nvarchar(40)  NULL,
    [RegisteredRoleId] bigint  NULL,
    [VerificationKey] nvarchar(128)  NULL
);
GO

-- Creating table 'UserMerchants'
CREATE TABLE [dbo].[UserMerchants] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [MerchantId] bigint  NULL,
    [Account] nvarchar(50)  NULL,
    [PreviousAccount] nvarchar(50)  NULL,
    [NextAccount] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [LastChangeDateTime] datetime  NULL
);
GO

-- Creating table 'Video'
CREATE TABLE [dbo].[Video] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NOT NULL,
    [VideoName] nvarchar(255)  NOT NULL,
    [VideoLink] nvarchar(4000)  NOT NULL,
    [VideoSize] bigint  NOT NULL,
    [VideoLength] bigint  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [Extension] nvarchar(40)  NULL
);
GO

-- Creating table 'Wallet'
CREATE TABLE [dbo].[Wallet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [Account] decimal(25,13)  NULL,
    [Transfer] decimal(25,13)  NULL,
    [ReferralsIncome] decimal(25,13)  NULL
);
GO

-- Creating table 'WhiteList'
CREATE TABLE [dbo].[WhiteList] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OwnerId] bigint  NULL,
    [UserId] bigint  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [OperationKey] in table 'C__RefactorLog'
ALTER TABLE [dbo].[C__RefactorLog]
ADD CONSTRAINT [PK_C__RefactorLog]
    PRIMARY KEY CLUSTERED ([OperationKey] ASC);
GO

-- Creating primary key on [Id] in table 'AlertMessage'
ALTER TABLE [dbo].[AlertMessage]
ADD CONSTRAINT [PK_AlertMessage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Ban'
ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [PK_Ban]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BlackList'
ALTER TABLE [dbo].[BlackList]
ADD CONSTRAINT [PK_BlackList]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Certificates'
ALTER TABLE [dbo].[Certificates]
ADD CONSTRAINT [PK_Certificates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Cheaters'
ALTER TABLE [dbo].[Cheaters]
ADD CONSTRAINT [PK_Cheaters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Geolocation'
ALTER TABLE [dbo].[Geolocation]
ADD CONSTRAINT [PK_Geolocation]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GeolocationPlatformPercentage'
ALTER TABLE [dbo].[GeolocationPlatformPercentage]
ADD CONSTRAINT [PK_GeolocationPlatformPercentage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Image'
ALTER TABLE [dbo].[Image]
ADD CONSTRAINT [PK_Image]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Merchants'
ALTER TABLE [dbo].[Merchants]
ADD CONSTRAINT [PK_Merchants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MoneyTransfers'
ALTER TABLE [dbo].[MoneyTransfers]
ADD CONSTRAINT [PK_MoneyTransfers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'News'
ALTER TABLE [dbo].[News]
ADD CONSTRAINT [PK_News]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [PK_Order]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderContent'
ALTER TABLE [dbo].[OrderContent]
ADD CONSTRAINT [PK_OrderContent]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderPerformed'
ALTER TABLE [dbo].[OrderPerformed]
ADD CONSTRAINT [PK_OrderPerformed]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderThemes'
ALTER TABLE [dbo].[OrderThemes]
ADD CONSTRAINT [PK_OrderThemes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PerformerPlatform'
ALTER TABLE [dbo].[PerformerPlatform]
ADD CONSTRAINT [PK_PerformerPlatform]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PerformerStatistics'
ALTER TABLE [dbo].[PerformerStatistics]
ADD CONSTRAINT [PK_PerformerStatistics]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Regions'
ALTER TABLE [dbo].[Regions]
ADD CONSTRAINT [PK_Regions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'Themes'
ALTER TABLE [dbo].[Themes]
ADD CONSTRAINT [PK_Themes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [PK_Tickets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TicketThemes'
ALTER TABLE [dbo].[TicketThemes]
ADD CONSTRAINT [PK_TicketThemes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Timezone'
ALTER TABLE [dbo].[Timezone]
ADD CONSTRAINT [PK_Timezone]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [PK_Transactions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserMerchants'
ALTER TABLE [dbo].[UserMerchants]
ADD CONSTRAINT [PK_UserMerchants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Video'
ALTER TABLE [dbo].[Video]
ADD CONSTRAINT [PK_Video]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Wallet'
ALTER TABLE [dbo].[Wallet]
ADD CONSTRAINT [PK_Wallet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WhiteList'
ALTER TABLE [dbo].[WhiteList]
ADD CONSTRAINT [PK_WhiteList]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CreatorId] in table 'AlertMessage'
ALTER TABLE [dbo].[AlertMessage]
ADD CONSTRAINT [FK_AlertMessage_User]
    FOREIGN KEY ([CreatorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AlertMessage_User'
CREATE INDEX [IX_FK_AlertMessage_User]
ON [dbo].[AlertMessage]
    ([CreatorId]);
GO

-- Creating foreign key on [RoleId] in table 'Ban'
ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ban_Roles'
CREATE INDEX [IX_FK_Ban_Roles]
ON [dbo].[Ban]
    ([RoleId]);
GO

-- Creating foreign key on [UserId] in table 'Ban'
ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ban_User'
CREATE INDEX [IX_FK_Ban_User]
ON [dbo].[Ban]
    ([UserId]);
GO

-- Creating foreign key on [AdminBanId] in table 'Ban'
ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_User1]
    FOREIGN KEY ([AdminBanId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ban_User1'
CREATE INDEX [IX_FK_Ban_User1]
ON [dbo].[Ban]
    ([AdminBanId]);
GO

-- Creating foreign key on [AdminUnbanId] in table 'Ban'
ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_User2]
    FOREIGN KEY ([AdminUnbanId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ban_User2'
CREATE INDEX [IX_FK_Ban_User2]
ON [dbo].[Ban]
    ([AdminUnbanId]);
GO

-- Creating foreign key on [OwnerId] in table 'BlackList'
ALTER TABLE [dbo].[BlackList]
ADD CONSTRAINT [FK_BlackList_User]
    FOREIGN KEY ([OwnerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BlackList_User'
CREATE INDEX [IX_FK_BlackList_User]
ON [dbo].[BlackList]
    ([OwnerId]);
GO

-- Creating foreign key on [UserId] in table 'BlackList'
ALTER TABLE [dbo].[BlackList]
ADD CONSTRAINT [FK_BlackList_User1]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BlackList_User1'
CREATE INDEX [IX_FK_BlackList_User1]
ON [dbo].[BlackList]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'Certificates'
ALTER TABLE [dbo].[Certificates]
ADD CONSTRAINT [FK_Certificates_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Certificates_User'
CREATE INDEX [IX_FK_Certificates_User]
ON [dbo].[Certificates]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'Cheaters'
ALTER TABLE [dbo].[Cheaters]
ADD CONSTRAINT [FK_Cheaters_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Cheaters_User'
CREATE INDEX [IX_FK_Cheaters_User]
ON [dbo].[Cheaters]
    ([UserId]);
GO

-- Creating foreign key on [GeolocationId] in table 'GeolocationPlatformPercentage'
ALTER TABLE [dbo].[GeolocationPlatformPercentage]
ADD CONSTRAINT [FK_GeolocationPlatformPercentage_Geolocation]
    FOREIGN KEY ([GeolocationId])
    REFERENCES [dbo].[Geolocation]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeolocationPlatformPercentage_Geolocation'
CREATE INDEX [IX_FK_GeolocationPlatformPercentage_Geolocation]
ON [dbo].[GeolocationPlatformPercentage]
    ([GeolocationId]);
GO

-- Creating foreign key on [UserId] in table 'Image'
ALTER TABLE [dbo].[Image]
ADD CONSTRAINT [FK_Image_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Image_User'
CREATE INDEX [IX_FK_Image_User]
ON [dbo].[Image]
    ([UserId]);
GO

-- Creating foreign key on [ImageId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_Image]
    FOREIGN KEY ([ImageId])
    REFERENCES [dbo].[Image]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ticket_Image'
CREATE INDEX [IX_FK_Ticket_Image]
ON [dbo].[Tickets]
    ([ImageId]);
GO

-- Creating foreign key on [MerchantId] in table 'MoneyTransfers'
ALTER TABLE [dbo].[MoneyTransfers]
ADD CONSTRAINT [FK_MoneyTransfers_Merchants]
    FOREIGN KEY ([MerchantId])
    REFERENCES [dbo].[Merchants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MoneyTransfers_Merchants'
CREATE INDEX [IX_FK_MoneyTransfers_Merchants]
ON [dbo].[MoneyTransfers]
    ([MerchantId]);
GO

-- Creating foreign key on [MerchantId] in table 'UserMerchants'
ALTER TABLE [dbo].[UserMerchants]
ADD CONSTRAINT [FK_UserMerchants_Merchants]
    FOREIGN KEY ([MerchantId])
    REFERENCES [dbo].[Merchants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMerchants_Merchants'
CREATE INDEX [IX_FK_UserMerchants_Merchants]
ON [dbo].[UserMerchants]
    ([MerchantId]);
GO

-- Creating foreign key on [FromUserId] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_Messages_User]
    FOREIGN KEY ([FromUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Messages_User'
CREATE INDEX [IX_FK_Messages_User]
ON [dbo].[Messages]
    ([FromUserId]);
GO

-- Creating foreign key on [ToUserId] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_Messages_User1]
    FOREIGN KEY ([ToUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Messages_User1'
CREATE INDEX [IX_FK_Messages_User1]
ON [dbo].[Messages]
    ([ToUserId]);
GO

-- Creating foreign key on [UserId] in table 'MoneyTransfers'
ALTER TABLE [dbo].[MoneyTransfers]
ADD CONSTRAINT [FK_MoneyTransfers_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MoneyTransfers_User'
CREATE INDEX [IX_FK_MoneyTransfers_User]
ON [dbo].[MoneyTransfers]
    ([UserId]);
GO

-- Creating foreign key on [CreatorId] in table 'News'
ALTER TABLE [dbo].[News]
ADD CONSTRAINT [FK_News_User]
    FOREIGN KEY ([CreatorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_News_User'
CREATE INDEX [IX_FK_News_User]
ON [dbo].[News]
    ([CreatorId]);
GO

-- Creating foreign key on [RegionId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_Order_Regions]
    FOREIGN KEY ([RegionId])
    REFERENCES [dbo].[Regions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Order_Regions'
CREATE INDEX [IX_FK_Order_Regions]
ON [dbo].[Order]
    ([RegionId]);
GO

-- Creating foreign key on [UserId] in table 'Order'
ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_Order_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Order_User'
CREATE INDEX [IX_FK_Order_User]
ON [dbo].[Order]
    ([UserId]);
GO

-- Creating foreign key on [OrderId] in table 'OrderThemes'
ALTER TABLE [dbo].[OrderThemes]
ADD CONSTRAINT [FK_OrderThemes_Order]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[Order]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderThemes_Order'
CREATE INDEX [IX_FK_OrderThemes_Order]
ON [dbo].[OrderThemes]
    ([OrderId]);
GO

-- Creating foreign key on [OrderContentId] in table 'OrderPerformed'
ALTER TABLE [dbo].[OrderPerformed]
ADD CONSTRAINT [FK_OrderPerformed_OrderImages]
    FOREIGN KEY ([OrderContentId])
    REFERENCES [dbo].[OrderContent]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderPerformed_OrderImages'
CREATE INDEX [IX_FK_OrderPerformed_OrderImages]
ON [dbo].[OrderPerformed]
    ([OrderContentId]);
GO

-- Creating foreign key on [AuthorId] in table 'OrderPerformed'
ALTER TABLE [dbo].[OrderPerformed]
ADD CONSTRAINT [FK_OrderPerformed_User]
    FOREIGN KEY ([AuthorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderPerformed_User'
CREATE INDEX [IX_FK_OrderPerformed_User]
ON [dbo].[OrderPerformed]
    ([AuthorId]);
GO

-- Creating foreign key on [ThemeId] in table 'OrderThemes'
ALTER TABLE [dbo].[OrderThemes]
ADD CONSTRAINT [FK_OrderThemes_Theme]
    FOREIGN KEY ([ThemeId])
    REFERENCES [dbo].[Themes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderThemes_Theme'
CREATE INDEX [IX_FK_OrderThemes_Theme]
ON [dbo].[OrderThemes]
    ([ThemeId]);
GO

-- Creating foreign key on [ThemeId] in table 'PerformerPlatform'
ALTER TABLE [dbo].[PerformerPlatform]
ADD CONSTRAINT [FK_PerformerPlatform_Themes]
    FOREIGN KEY ([ThemeId])
    REFERENCES [dbo].[Themes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PerformerPlatform_Themes'
CREATE INDEX [IX_FK_PerformerPlatform_Themes]
ON [dbo].[PerformerPlatform]
    ([ThemeId]);
GO

-- Creating foreign key on [PerformerId] in table 'PerformerPlatform'
ALTER TABLE [dbo].[PerformerPlatform]
ADD CONSTRAINT [FK_PerformerPlatform_User]
    FOREIGN KEY ([PerformerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PerformerPlatform_User'
CREATE INDEX [IX_FK_PerformerPlatform_User]
ON [dbo].[PerformerPlatform]
    ([PerformerId]);
GO

-- Creating foreign key on [PerformerPlatformId] in table 'PerformerStatistics'
ALTER TABLE [dbo].[PerformerStatistics]
ADD CONSTRAINT [FK_PerformerStatistics_GeolocationPlatformPercentage]
    FOREIGN KEY ([PerformerPlatformId])
    REFERENCES [dbo].[PerformerPlatform]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PerformerStatistics_GeolocationPlatformPercentage'
CREATE INDEX [IX_FK_PerformerStatistics_GeolocationPlatformPercentage]
ON [dbo].[PerformerStatistics]
    ([PerformerPlatformId]);
GO

-- Creating foreign key on [RoleId] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_User_Roles'
CREATE INDEX [IX_FK_User_Roles]
ON [dbo].[User]
    ([RoleId]);
GO

-- Creating foreign key on [ParentTicketId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_Ticket]
    FOREIGN KEY ([ParentTicketId])
    REFERENCES [dbo].[Tickets]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ticket_Ticket'
CREATE INDEX [IX_FK_Ticket_Ticket]
ON [dbo].[Tickets]
    ([ParentTicketId]);
GO

-- Creating foreign key on [ThemeId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_TicketThemes]
    FOREIGN KEY ([ThemeId])
    REFERENCES [dbo].[TicketThemes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ticket_TicketThemes'
CREATE INDEX [IX_FK_Ticket_TicketThemes]
ON [dbo].[Tickets]
    ([ThemeId]);
GO

-- Creating foreign key on [CreatorId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_User]
    FOREIGN KEY ([CreatorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ticket_User'
CREATE INDEX [IX_FK_Ticket_User]
ON [dbo].[Tickets]
    ([CreatorId]);
GO

-- Creating foreign key on [AnswerAdminId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_User1]
    FOREIGN KEY ([AnswerAdminId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ticket_User1'
CREATE INDEX [IX_FK_Ticket_User1]
ON [dbo].[Tickets]
    ([AnswerAdminId]);
GO

-- Creating foreign key on [CloseAdminId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_User2]
    FOREIGN KEY ([CloseAdminId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Ticket_User2'
CREATE INDEX [IX_FK_Ticket_User2]
ON [dbo].[Tickets]
    ([CloseAdminId]);
GO

-- Creating foreign key on [TimeZoneId] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_Timezone]
    FOREIGN KEY ([TimeZoneId])
    REFERENCES [dbo].[Timezone]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_User_Timezone'
CREATE INDEX [IX_FK_User_Timezone]
ON [dbo].[User]
    ([TimeZoneId]);
GO

-- Creating foreign key on [FromUserId] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_Transactions_User]
    FOREIGN KEY ([FromUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Transactions_User'
CREATE INDEX [IX_FK_Transactions_User]
ON [dbo].[Transactions]
    ([FromUserId]);
GO

-- Creating foreign key on [ToUserId] in table 'Transactions'
ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_Transactions_User1]
    FOREIGN KEY ([ToUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Transactions_User1'
CREATE INDEX [IX_FK_Transactions_User1]
ON [dbo].[Transactions]
    ([ToUserId]);
GO

-- Creating foreign key on [ReferrerId] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_User]
    FOREIGN KEY ([ReferrerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_User_User'
CREATE INDEX [IX_FK_User_User]
ON [dbo].[User]
    ([ReferrerId]);
GO

-- Creating foreign key on [UserId] in table 'UserMerchants'
ALTER TABLE [dbo].[UserMerchants]
ADD CONSTRAINT [FK_UserMerchants_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMerchants_User'
CREATE INDEX [IX_FK_UserMerchants_User]
ON [dbo].[UserMerchants]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'Video'
ALTER TABLE [dbo].[Video]
ADD CONSTRAINT [FK_Video_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Video_User'
CREATE INDEX [IX_FK_Video_User]
ON [dbo].[Video]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'Wallet'
ALTER TABLE [dbo].[Wallet]
ADD CONSTRAINT [FK_Wallet_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Wallet_User'
CREATE INDEX [IX_FK_Wallet_User]
ON [dbo].[Wallet]
    ([UserId]);
GO

-- Creating foreign key on [OwnerId] in table 'WhiteList'
ALTER TABLE [dbo].[WhiteList]
ADD CONSTRAINT [FK_WhiteList_User]
    FOREIGN KEY ([OwnerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WhiteList_User'
CREATE INDEX [IX_FK_WhiteList_User]
ON [dbo].[WhiteList]
    ([OwnerId]);
GO

-- Creating foreign key on [UserId] in table 'WhiteList'
ALTER TABLE [dbo].[WhiteList]
ADD CONSTRAINT [FK_WhiteList_User1]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WhiteList_User1'
CREATE INDEX [IX_FK_WhiteList_User1]
ON [dbo].[WhiteList]
    ([UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------