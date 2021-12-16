/*
Скрипт развертывания для Vodad

Этот код был создан программным средством.
Изменения, внесенные в этот файл, могут привести к неверному выполнению кода и будут потеряны
в случае его повторного формирования.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Vodad"
:setvar DefaultFilePrefix "Vodad"
:setvar DefaultDataPath "D:\SQLServers\SQL2012CSAS\MSSQL11.SQL2012CSAS\MSSQL\DATA\"
:setvar DefaultLogPath "D:\SQLServers\SQL2012CSAS\MSSQL11.SQL2012CSAS\MSSQL\DATA\"

GO
:on error exit
GO
/*
Проверьте режим SQLCMD и отключите выполнение скрипта, если режим SQLCMD не поддерживается.
Чтобы повторно включить скрипт после включения режима SQLCMD выполните следующую инструкцию:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'Для успешного выполнения этого скрипта должен быть включен режим SQLCMD.';
        SET NOEXEC ON;
    END


GO
USE [master];


GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Выполняется создание $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)]
    ON 
    PRIMARY(NAME = [$(DatabaseName)], FILENAME = N'$(DefaultDataPath)$(DefaultFilePrefix)_Primary.mdf')
    LOG ON (NAME = [$(DatabaseName)_log], FILENAME = N'$(DefaultLogPath)$(DefaultFilePrefix)_Primary.ldf') COLLATE SQL_Latin1_General_CP1_CI_AS
GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'Параметры базы данных изменить нельзя. Применить эти параметры может только пользователь SysAdmin.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'Параметры базы данных изменить нельзя. Применить эти параметры может только пользователь SysAdmin.';
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET TARGET_RECOVERY_TIME = 0 SECONDS 
    WITH ROLLBACK IMMEDIATE;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF),
                CONTAINMENT = NONE 
            WITH ROLLBACK IMMEDIATE;
    END


GO
USE [$(DatabaseName)];


GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
GO

GO
PRINT N'Выполняется создание [dbo].[AlertMessage]...';


GO
CREATE TABLE [dbo].[AlertMessage] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreatorId]    BIGINT         NULL,
    [Text]         NVARCHAR (500) NULL,
    [CreationDate] DATETIME       NULL,
    CONSTRAINT [PK_AlertMessage] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[AlertMessage].[IX_FK_AlertMessage_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_AlertMessage_User]
    ON [dbo].[AlertMessage]([CreatorId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Ban]...';


GO
CREATE TABLE [dbo].[Ban] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [UserId]        BIGINT          NULL,
    [RoleId]        BIGINT          NULL,
    [BanDateTime]   DATETIME        NULL,
    [UnbanDateTime] DATETIME        NULL,
    [BanReason]     NVARCHAR (4000) NULL,
    [UnbanReason]   NVARCHAR (4000) NULL,
    [CanBeUnbanned] BIT             NULL,
    [AdminBanId]    BIGINT          NULL,
    [AdminUnbanId]  BIGINT          NULL,
    CONSTRAINT [PK_Ban] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Ban].[IX_FK_Ban_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ban_User]
    ON [dbo].[Ban]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Ban].[IX_FK_Ban_Roles]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ban_Roles]
    ON [dbo].[Ban]([RoleId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Ban].[IX_FK_Ban_User1]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ban_User1]
    ON [dbo].[Ban]([AdminBanId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Ban].[IX_FK_Ban_User2]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ban_User2]
    ON [dbo].[Ban]([AdminUnbanId] ASC);


GO
PRINT N'Выполняется создание [dbo].[BlackList]...';


GO
CREATE TABLE [dbo].[BlackList] (
    [Id]      BIGINT IDENTITY (1, 1) NOT NULL,
    [OwnerId] BIGINT NULL,
    [UserId]  BIGINT NULL,
    CONSTRAINT [PK_BlackList] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[BlackList].[IX_FK_BlackList_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_BlackList_User]
    ON [dbo].[BlackList]([OwnerId] ASC);


GO
PRINT N'Выполняется создание [dbo].[BlackList].[IX_FK_BlackList_User1]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_BlackList_User1]
    ON [dbo].[BlackList]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Certificates]...';


GO
CREATE TABLE [dbo].[Certificates] (
    [Id]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserId] BIGINT        NULL,
    [Code]   NVARCHAR (50) NULL,
    [Status] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Certificates] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Certificates].[IX_FK_Certificates_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Certificates_User]
    ON [dbo].[Certificates]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Cheaters]...';


GO
CREATE TABLE [dbo].[Cheaters] (
    [Id]     BIGINT IDENTITY (1, 1) NOT NULL,
    [UserId] BIGINT NULL,
    CONSTRAINT [PK_Cheaters] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Cheaters].[IX_FK_Cheaters_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Cheaters_User]
    ON [dbo].[Cheaters]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Geolocation]...';


GO
CREATE TABLE [dbo].[Geolocation] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [CountryName]     NVARCHAR (50)  NULL,
    [ISO2]            NVARCHAR (MAX) NULL,
    [LongCountryName] NVARCHAR (MAX) NULL,
    [ISO3]            NVARCHAR (MAX) NULL,
    [NumCode]         NVARCHAR (MAX) NULL,
    [UNMemberState]   NVARCHAR (MAX) NULL,
    [CallingCode]     NVARCHAR (MAX) NULL,
    [CCTLD]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Geolocation] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[GeolocationPlatformPercentage]...';


GO
CREATE TABLE [dbo].[GeolocationPlatformPercentage] (
    [Id]                  BIGINT IDENTITY (1, 1) NOT NULL,
    [GeolocationId]       BIGINT NULL,
    [Percentage]          BIGINT NULL,
    [PerformerPlatformId] BIGINT NOT NULL,
    CONSTRAINT [PK_GeolocationPlatformPercentage] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[GeolocationPlatformPercentage].[IX_FK_GeolocationPlatformPercentage_Geolocation]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_GeolocationPlatformPercentage_Geolocation]
    ON [dbo].[GeolocationPlatformPercentage]([GeolocationId] ASC);


GO
PRINT N'Выполняется создание [dbo].[GeolocationPlatformPercentage].[IX_FK_GeolocationPlatformPercentagePerformerPlatform]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_GeolocationPlatformPercentagePerformerPlatform]
    ON [dbo].[GeolocationPlatformPercentage]([PerformerPlatformId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Image]...';


GO
CREATE TABLE [dbo].[Image] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [UserId]       BIGINT          NOT NULL,
    [ImageName]    NVARCHAR (255)  NOT NULL,
    [ImageData]    VARBINARY (MAX) NOT NULL,
    [ImageSize]    BIGINT          NOT NULL,
    [CreationDate] DATETIME        NOT NULL,
    [ImageWidth]   INT             NULL,
    [ImageHeight]  INT             NULL,
    [Extension]    NVARCHAR (40)   NULL,
    CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Image].[IX_FK_Image_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Image_User]
    ON [dbo].[Image]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Merchants]...';


GO
CREATE TABLE [dbo].[Merchants] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [MerchantName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Merchants] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Messages]...';


GO
CREATE TABLE [dbo].[Messages] (
    [Id]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [FromUserId]           BIGINT          NULL,
    [ToUserId]             BIGINT          NULL,
    [CreationDate]         DATETIME        NULL,
    [MessageText]          NVARCHAR (4000) NULL,
    [IsRead]               BIT             NULL,
    [MessageTitle]         NVARCHAR (50)   NULL,
    [IsDeletedForAuthor]   BIT             NULL,
    [IsDeletedForReciever] BIT             NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Messages].[IX_FK_Messages_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Messages_User]
    ON [dbo].[Messages]([FromUserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Messages].[IX_FK_Messages_User1]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Messages_User1]
    ON [dbo].[Messages]([ToUserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[MoneyTransfers]...';


GO
CREATE TABLE [dbo].[MoneyTransfers] (
    [Id]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]                BIGINT           NULL,
    [DateTime]              DATETIME         NULL,
    [Action]                NVARCHAR (50)    NULL,
    [Amount]                DECIMAL (25, 13) NULL,
    [MerchantId]            BIGINT           NULL,
    [AccountMerchantSystem] NVARCHAR (50)    NULL,
    [TransactionId]         NVARCHAR (50)    NULL,
    [Status]                NVARCHAR (50)    NULL,
    [StatusDescription]     NVARCHAR (4000)  NULL,
    CONSTRAINT [PK_MoneyTransfers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[MoneyTransfers].[IX_FK_MoneyTransfers_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_MoneyTransfers_User]
    ON [dbo].[MoneyTransfers]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[MoneyTransfers].[IX_FK_MoneyTransfers_Merchants]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_MoneyTransfers_Merchants]
    ON [dbo].[MoneyTransfers]([MerchantId] ASC);


GO
PRINT N'Выполняется создание [dbo].[News]...';


GO
CREATE TABLE [dbo].[News] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [CreatorId]    BIGINT          NULL,
    [Title]        NVARCHAR (100)  NULL,
    [Text]         NVARCHAR (4000) NULL,
    [Text1]        NVARCHAR (4000) NULL,
    [Text2]        NVARCHAR (4000) NULL,
    [CreationDate] DATETIME        NULL,
    CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[News].[IX_FK_News_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_News_User]
    ON [dbo].[News]([CreatorId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Order]...';


GO
CREATE TABLE [dbo].[Order] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [UserId]       BIGINT          NULL,
    [Comment]      NVARCHAR (4000) NULL,
    [CreationDate] DATETIME        NULL,
    [ExpireDate]   DATETIME        NULL,
    [Status]       NVARCHAR (50)   NULL,
    [RegionId]     BIGINT          NULL,
    [Name]         NVARCHAR (100)  NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Order].[IX_FK_Order_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Order_User]
    ON [dbo].[Order]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Order].[IX_FK_Order_Regions]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Order_Regions]
    ON [dbo].[Order]([RegionId] ASC);


GO
PRINT N'Выполняется создание [dbo].[OrderContent]...';


GO
CREATE TABLE [dbo].[OrderContent] (
    [Id]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [IdOrder]     BIGINT        NOT NULL,
    [IdContent]   BIGINT        NOT NULL,
    [Status]      NVARCHAR (50) NULL,
    [ContentType] NVARCHAR (50) NULL,
    CONSTRAINT [PK_OrderContent] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[OrderContent].[IX_FK_OrderContent_Order]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderContent_Order]
    ON [dbo].[OrderContent]([IdOrder] ASC);


GO
PRINT N'Выполняется создание [dbo].[OrderPerformed]...';


GO
CREATE TABLE [dbo].[OrderPerformed] (
    [Id]                       BIGINT           IDENTITY (1, 1) NOT NULL,
    [Status]                   NVARCHAR (50)    NULL,
    [MoneyPaid]                DECIMAL (25, 13) NULL,
    [OrderContentId]           BIGINT           NULL,
    [AuthorId]                 BIGINT           NOT NULL,
    [IsLiked]                  BIT              NULL,
    [PerformerPlatformId]      BIGINT           NULL,
    [VideoLink]                NVARCHAR (400)   NULL,
    [StartDate]                DATETIME         NULL,
    [LastStatusChangeDateTime] DATETIME         NULL,
    CONSTRAINT [PK_OrderPerformed] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[OrderPerformed].[IX_FK_OrderPerformed_OrderContent]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderPerformed_OrderContent]
    ON [dbo].[OrderPerformed]([OrderContentId] ASC);


GO
PRINT N'Выполняется создание [dbo].[OrderPerformed].[IX_FK_OrderPerformed_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderPerformed_User]
    ON [dbo].[OrderPerformed]([AuthorId] ASC);


GO
PRINT N'Выполняется создание [dbo].[OrderPerformed].[IX_FK_OrderPerformed_PerformerPlatform]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderPerformed_PerformerPlatform]
    ON [dbo].[OrderPerformed]([PerformerPlatformId] ASC);


GO
PRINT N'Выполняется создание [dbo].[OrderThemes]...';


GO
CREATE TABLE [dbo].[OrderThemes] (
    [Id]      BIGINT IDENTITY (1, 1) NOT NULL,
    [OrderId] BIGINT NULL,
    [ThemeId] BIGINT NULL,
    CONSTRAINT [PK_OrderThemes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[OrderThemes].[IX_FK_OrderThemes_Order]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderThemes_Order]
    ON [dbo].[OrderThemes]([OrderId] ASC);


GO
PRINT N'Выполняется создание [dbo].[OrderThemes].[IX_FK_OrderThemes_Theme]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_OrderThemes_Theme]
    ON [dbo].[OrderThemes]([ThemeId] ASC);


GO
PRINT N'Выполняется создание [dbo].[PerformerPlatform]...';


GO
CREATE TABLE [dbo].[PerformerPlatform] (
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [PerformerId]        BIGINT         NULL,
    [Login]              NVARCHAR (50)  NULL,
    [Password]           NVARCHAR (50)  NULL,
    [Status]             NVARCHAR (50)  NULL,
    [ThemeId]            BIGINT         NULL,
    [Verified]           NVARCHAR (50)  NULL,
    [LastOnlineDateTime] DATETIME       NULL,
    [Link]               NVARCHAR (200) NULL,
    [Date]               DATETIME       NULL,
    [ChannelName]        NVARCHAR (50)  NULL,
    CONSTRAINT [PK_PerformerPlatform] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[PerformerPlatform].[IX_FK_PerformerPlatform_Themes]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_PerformerPlatform_Themes]
    ON [dbo].[PerformerPlatform]([ThemeId] ASC);


GO
PRINT N'Выполняется создание [dbo].[PerformerPlatform].[IX_FK_PerformerPlatform_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_PerformerPlatform_User]
    ON [dbo].[PerformerPlatform]([PerformerId] ASC);


GO
PRINT N'Выполняется создание [dbo].[PerformerStatistics]...';


GO
CREATE TABLE [dbo].[PerformerStatistics] (
    [Id]                        BIGINT         IDENTITY (1, 1) NOT NULL,
    [PerformerPlatformId]       BIGINT         NULL,
    [AverageViewerCountPerHour] BIGINT         NULL,
    [MaxViewersCount]           BIGINT         NULL,
    [TotalUniqueViews]          BIGINT         NULL,
    [TotalFollowers]            BIGINT         NULL,
    [TotalViews]                DECIMAL (18)   NULL,
    [Likes]                     BIGINT         NULL,
    [CompletedOrders]           BIGINT         NULL,
    [TotalOrders]               BIGINT         NULL,
    [AverageComplitionSpeed]    DECIMAL (18)   NULL,
    [DateModified]              DATETIME       NOT NULL,
    [Status]                    NVARCHAR (MAX) NULL,
    [UniqueViewersForMonth]     BIGINT         NULL,
    CONSTRAINT [PK_PerformerStatistics] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[PerformerStatistics].[IX_FK_PerformerStatistics_GeolocationPlatformPercentage]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_PerformerStatistics_GeolocationPlatformPercentage]
    ON [dbo].[PerformerStatistics]([PerformerPlatformId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Regions]...';


GO
CREATE TABLE [dbo].[Regions] (
    [Id]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [RegionName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Roles]...';


GO
CREATE TABLE [dbo].[Roles] (
    [Id]       BIGINT        IDENTITY (1, 1) NOT NULL,
    [RoleName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Themes]...';


GO
CREATE TABLE [dbo].[Themes] (
    [Id]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Themes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Tickets]...';


GO
CREATE TABLE [dbo].[Tickets] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [Title]             NVARCHAR (500)  NULL,
    [ThemeId]           BIGINT          NULL,
    [Text1]             NVARCHAR (4000) NULL,
    [Text2]             NVARCHAR (4000) NULL,
    [Text3]             NVARCHAR (4000) NULL,
    [ImageId]           BIGINT          NULL,
    [CreationDate]      DATETIME        NULL,
    [AnswerDate]        DATETIME        NULL,
    [CloseDate]         DATETIME        NULL,
    [ParentTicketId]    BIGINT          NULL,
    [AdminAnswer]       NVARCHAR (4000) NULL,
    [AdminCloseComment] NVARCHAR (4000) NULL,
    [CreatorId]         BIGINT          NULL,
    [AnswerAdminId]     BIGINT          NULL,
    [CloseAdminId]      BIGINT          NULL,
    [Status]            NVARCHAR (50)   NULL,
    CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Tickets].[IX_FK_Ticket_Image]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ticket_Image]
    ON [dbo].[Tickets]([ImageId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Tickets].[IX_FK_Ticket_Ticket]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ticket_Ticket]
    ON [dbo].[Tickets]([ParentTicketId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Tickets].[IX_FK_Ticket_TicketThemes]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ticket_TicketThemes]
    ON [dbo].[Tickets]([ThemeId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Tickets].[IX_FK_Ticket_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ticket_User]
    ON [dbo].[Tickets]([CreatorId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Tickets].[IX_FK_Ticket_User1]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ticket_User1]
    ON [dbo].[Tickets]([AnswerAdminId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Tickets].[IX_FK_Ticket_User2]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Ticket_User2]
    ON [dbo].[Tickets]([CloseAdminId] ASC);


GO
PRINT N'Выполняется создание [dbo].[TicketThemes]...';


GO
CREATE TABLE [dbo].[TicketThemes] (
    [Id]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [ThemeName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_TicketThemes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Timezone]...';


GO
CREATE TABLE [dbo].[Timezone] (
    [Id]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (100) NULL,
    CONSTRAINT [PK_Timezone] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Transactions]...';


GO
CREATE TABLE [dbo].[Transactions] (
    [Id]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [FromUserId] BIGINT           NOT NULL,
    [Amount]     DECIMAL (24, 13) NOT NULL,
    [ToUserId]   BIGINT           NOT NULL,
    [DateTime]   DATETIME         NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Transactions].[IX_FK_Transactions_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Transactions_User]
    ON [dbo].[Transactions]([FromUserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Transactions].[IX_FK_Transactions_User1]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Transactions_User1]
    ON [dbo].[Transactions]([ToUserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[User]...';


GO
CREATE TABLE [dbo].[User] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (50)   NULL,
    [Login]                 NVARCHAR (50)   NULL,
    [Email]                 NVARCHAR (100)  NULL,
    [Password]              NVARCHAR (128)  NULL,
    [RoleId]                BIGINT          NULL,
    [RegistrationDate]      DATETIME        NULL,
    [TimeZoneId]            BIGINT          NULL,
    [ReferrerId]            BIGINT          NULL,
    [Status]                NVARCHAR (50)   NULL,
    [PasswordSalt]          NVARCHAR (128)  NOT NULL,
    [Comments]              NVARCHAR (4000) NULL,
    [LastModifiedDate]      DATETIME        NULL,
    [LastLoginDate]         DATETIME        NOT NULL,
    [LastLoginIp]           NVARCHAR (40)   NULL,
    [IsActivated]           BIT             NOT NULL,
    [IsLockedOut]           BIT             NOT NULL,
    [LastLockedOutDate]     DATETIME        NOT NULL,
    [LastLockedOutReason]   NVARCHAR (256)  NULL,
    [NewPasswordKey]        NVARCHAR (128)  NULL,
    [NewEmail]              NVARCHAR (100)  NULL,
    [NewEmailKey]           NVARCHAR (128)  NULL,
    [NewEmailRequested]     DATETIME        NULL,
    [Rating]                BIGINT          NULL,
    [Karma]                 BIGINT          NULL,
    [LastOnlineDateTime]    DATETIME        NULL,
    [LastEmailSendDateTime] DATETIME        NULL,
    [AccountAttachedIP]     NVARCHAR (40)   NULL,
    [RegisteredRoleId]      BIGINT          NULL,
    [VerificationKey]       NVARCHAR (128)  NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[User].[IX_FK_User_Roles]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_User_Roles]
    ON [dbo].[User]([RoleId] ASC);


GO
PRINT N'Выполняется создание [dbo].[User].[IX_FK_User_Timezone]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_User_Timezone]
    ON [dbo].[User]([TimeZoneId] ASC);


GO
PRINT N'Выполняется создание [dbo].[User].[IX_FK_User_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_User_User]
    ON [dbo].[User]([ReferrerId] ASC);


GO
PRINT N'Выполняется создание [dbo].[UserMerchants]...';


GO
CREATE TABLE [dbo].[UserMerchants] (
    [Id]                 BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserId]             BIGINT        NULL,
    [MerchantId]         BIGINT        NULL,
    [Account]            NVARCHAR (50) NULL,
    [PreviousAccount]    NVARCHAR (50) NULL,
    [NextAccount]        NVARCHAR (50) NULL,
    [Status]             NVARCHAR (50) NULL,
    [LastChangeDateTime] DATETIME      NULL,
    CONSTRAINT [PK_UserMerchants] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[UserMerchants].[IX_FK_UserMerchants_Merchants]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserMerchants_Merchants]
    ON [dbo].[UserMerchants]([MerchantId] ASC);


GO
PRINT N'Выполняется создание [dbo].[UserMerchants].[IX_FK_UserMerchants_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserMerchants_User]
    ON [dbo].[UserMerchants]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Video]...';


GO
CREATE TABLE [dbo].[Video] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [UserId]       BIGINT          NOT NULL,
    [VideoName]    NVARCHAR (255)  NOT NULL,
    [VideoLink]    NVARCHAR (4000) NOT NULL,
    [VideoSize]    BIGINT          NOT NULL,
    [VideoLength]  BIGINT          NOT NULL,
    [CreationDate] DATETIME        NOT NULL,
    [Extension]    NVARCHAR (40)   NULL,
    CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Video].[IX_FK_Video_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Video_User]
    ON [dbo].[Video]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[Wallet]...';


GO
CREATE TABLE [dbo].[Wallet] (
    [Id]              BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]          BIGINT           NULL,
    [Account]         DECIMAL (25, 13) NULL,
    [Transfer]        DECIMAL (25, 13) NULL,
    [ReferralsIncome] DECIMAL (25, 13) NULL,
    CONSTRAINT [PK_Wallet] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[Wallet].[IX_FK_Wallet_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_Wallet_User]
    ON [dbo].[Wallet]([UserId] ASC);


GO
PRINT N'Выполняется создание [dbo].[WhiteList]...';


GO
CREATE TABLE [dbo].[WhiteList] (
    [Id]      BIGINT IDENTITY (1, 1) NOT NULL,
    [OwnerId] BIGINT NULL,
    [UserId]  BIGINT NULL,
    CONSTRAINT [PK_WhiteList] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Выполняется создание [dbo].[WhiteList].[IX_FK_WhiteList_User]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_WhiteList_User]
    ON [dbo].[WhiteList]([OwnerId] ASC);


GO
PRINT N'Выполняется создание [dbo].[WhiteList].[IX_FK_WhiteList_User1]...';


GO
CREATE NONCLUSTERED INDEX [IX_FK_WhiteList_User1]
    ON [dbo].[WhiteList]([UserId] ASC);


GO
PRINT N'Выполняется создание FK_AlertMessage_User...';


GO
ALTER TABLE [dbo].[AlertMessage] WITH NOCHECK
    ADD CONSTRAINT [FK_AlertMessage_User] FOREIGN KEY ([CreatorId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Ban_User...';


GO
ALTER TABLE [dbo].[Ban] WITH NOCHECK
    ADD CONSTRAINT [FK_Ban_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Ban_Roles...';


GO
ALTER TABLE [dbo].[Ban] WITH NOCHECK
    ADD CONSTRAINT [FK_Ban_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]);


GO
PRINT N'Выполняется создание FK_Ban_User1...';


GO
ALTER TABLE [dbo].[Ban] WITH NOCHECK
    ADD CONSTRAINT [FK_Ban_User1] FOREIGN KEY ([AdminBanId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Ban_User2...';


GO
ALTER TABLE [dbo].[Ban] WITH NOCHECK
    ADD CONSTRAINT [FK_Ban_User2] FOREIGN KEY ([AdminUnbanId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_BlackList_User...';


GO
ALTER TABLE [dbo].[BlackList] WITH NOCHECK
    ADD CONSTRAINT [FK_BlackList_User] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_BlackList_User1...';


GO
ALTER TABLE [dbo].[BlackList] WITH NOCHECK
    ADD CONSTRAINT [FK_BlackList_User1] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Certificates_User...';


GO
ALTER TABLE [dbo].[Certificates] WITH NOCHECK
    ADD CONSTRAINT [FK_Certificates_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Cheaters_User...';


GO
ALTER TABLE [dbo].[Cheaters] WITH NOCHECK
    ADD CONSTRAINT [FK_Cheaters_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_GeolocationPlatformPercentage_Geolocation...';


GO
ALTER TABLE [dbo].[GeolocationPlatformPercentage] WITH NOCHECK
    ADD CONSTRAINT [FK_GeolocationPlatformPercentage_Geolocation] FOREIGN KEY ([GeolocationId]) REFERENCES [dbo].[Geolocation] ([Id]);


GO
PRINT N'Выполняется создание FK_GeolocationPlatformPercentagePerformerPlatform...';


GO
ALTER TABLE [dbo].[GeolocationPlatformPercentage] WITH NOCHECK
    ADD CONSTRAINT [FK_GeolocationPlatformPercentagePerformerPlatform] FOREIGN KEY ([PerformerPlatformId]) REFERENCES [dbo].[PerformerPlatform] ([Id]);


GO
PRINT N'Выполняется создание FK_Image_User...';


GO
ALTER TABLE [dbo].[Image] WITH NOCHECK
    ADD CONSTRAINT [FK_Image_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Messages_User...';


GO
ALTER TABLE [dbo].[Messages] WITH NOCHECK
    ADD CONSTRAINT [FK_Messages_User] FOREIGN KEY ([FromUserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Messages_User1...';


GO
ALTER TABLE [dbo].[Messages] WITH NOCHECK
    ADD CONSTRAINT [FK_Messages_User1] FOREIGN KEY ([ToUserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_MoneyTransfers_User...';


GO
ALTER TABLE [dbo].[MoneyTransfers] WITH NOCHECK
    ADD CONSTRAINT [FK_MoneyTransfers_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_MoneyTransfers_Merchants...';


GO
ALTER TABLE [dbo].[MoneyTransfers] WITH NOCHECK
    ADD CONSTRAINT [FK_MoneyTransfers_Merchants] FOREIGN KEY ([MerchantId]) REFERENCES [dbo].[Merchants] ([Id]);


GO
PRINT N'Выполняется создание FK_News_User...';


GO
ALTER TABLE [dbo].[News] WITH NOCHECK
    ADD CONSTRAINT [FK_News_User] FOREIGN KEY ([CreatorId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Order_User...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Order_Regions...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_Order_Regions] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Regions] ([Id]);


GO
PRINT N'Выполняется создание FK_OrderContent_Order...';


GO
ALTER TABLE [dbo].[OrderContent] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderContent_Order] FOREIGN KEY ([IdOrder]) REFERENCES [dbo].[Order] ([Id]);


GO
PRINT N'Выполняется создание FK_OrderPerformed_OrderImages...';


GO
ALTER TABLE [dbo].[OrderPerformed] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderPerformed_OrderImages] FOREIGN KEY ([OrderContentId]) REFERENCES [dbo].[OrderContent] ([Id]);


GO
PRINT N'Выполняется создание FK_OrderPerformed_User...';


GO
ALTER TABLE [dbo].[OrderPerformed] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderPerformed_User] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_OrderPerformed_PerformerPlatform...';


GO
ALTER TABLE [dbo].[OrderPerformed] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderPerformed_PerformerPlatform] FOREIGN KEY ([PerformerPlatformId]) REFERENCES [dbo].[PerformerPlatform] ([Id]);


GO
PRINT N'Выполняется создание FK_OrderThemes_Order...';


GO
ALTER TABLE [dbo].[OrderThemes] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderThemes_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]);


GO
PRINT N'Выполняется создание FK_OrderThemes_Theme...';


GO
ALTER TABLE [dbo].[OrderThemes] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderThemes_Theme] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[Themes] ([Id]);


GO
PRINT N'Выполняется создание FK_PerformerPlatform_Themes...';


GO
ALTER TABLE [dbo].[PerformerPlatform] WITH NOCHECK
    ADD CONSTRAINT [FK_PerformerPlatform_Themes] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[Themes] ([Id]);


GO
PRINT N'Выполняется создание FK_PerformerPlatform_User...';


GO
ALTER TABLE [dbo].[PerformerPlatform] WITH NOCHECK
    ADD CONSTRAINT [FK_PerformerPlatform_User] FOREIGN KEY ([PerformerId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_PerformerStatistics_GeolocationPlatformPercentage...';


GO
ALTER TABLE [dbo].[PerformerStatistics] WITH NOCHECK
    ADD CONSTRAINT [FK_PerformerStatistics_GeolocationPlatformPercentage] FOREIGN KEY ([PerformerPlatformId]) REFERENCES [dbo].[PerformerPlatform] ([Id]);


GO
PRINT N'Выполняется создание FK_Ticket_Image...';


GO
ALTER TABLE [dbo].[Tickets] WITH NOCHECK
    ADD CONSTRAINT [FK_Ticket_Image] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Image] ([Id]);


GO
PRINT N'Выполняется создание FK_Ticket_Ticket...';


GO
ALTER TABLE [dbo].[Tickets] WITH NOCHECK
    ADD CONSTRAINT [FK_Ticket_Ticket] FOREIGN KEY ([ParentTicketId]) REFERENCES [dbo].[Tickets] ([Id]);


GO
PRINT N'Выполняется создание FK_Ticket_TicketThemes...';


GO
ALTER TABLE [dbo].[Tickets] WITH NOCHECK
    ADD CONSTRAINT [FK_Ticket_TicketThemes] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[TicketThemes] ([Id]);


GO
PRINT N'Выполняется создание FK_Ticket_User...';


GO
ALTER TABLE [dbo].[Tickets] WITH NOCHECK
    ADD CONSTRAINT [FK_Ticket_User] FOREIGN KEY ([CreatorId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Ticket_User1...';


GO
ALTER TABLE [dbo].[Tickets] WITH NOCHECK
    ADD CONSTRAINT [FK_Ticket_User1] FOREIGN KEY ([AnswerAdminId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Ticket_User2...';


GO
ALTER TABLE [dbo].[Tickets] WITH NOCHECK
    ADD CONSTRAINT [FK_Ticket_User2] FOREIGN KEY ([CloseAdminId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Transactions_User...';


GO
ALTER TABLE [dbo].[Transactions] WITH NOCHECK
    ADD CONSTRAINT [FK_Transactions_User] FOREIGN KEY ([FromUserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Transactions_User1...';


GO
ALTER TABLE [dbo].[Transactions] WITH NOCHECK
    ADD CONSTRAINT [FK_Transactions_User1] FOREIGN KEY ([ToUserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_User_Roles...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]);


GO
PRINT N'Выполняется создание FK_User_Timezone...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_Timezone] FOREIGN KEY ([TimeZoneId]) REFERENCES [dbo].[Timezone] ([Id]);


GO
PRINT N'Выполняется создание FK_User_User...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_User] FOREIGN KEY ([ReferrerId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_UserMerchants_Merchants...';


GO
ALTER TABLE [dbo].[UserMerchants] WITH NOCHECK
    ADD CONSTRAINT [FK_UserMerchants_Merchants] FOREIGN KEY ([MerchantId]) REFERENCES [dbo].[Merchants] ([Id]);


GO
PRINT N'Выполняется создание FK_UserMerchants_User...';


GO
ALTER TABLE [dbo].[UserMerchants] WITH NOCHECK
    ADD CONSTRAINT [FK_UserMerchants_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Video_User...';


GO
ALTER TABLE [dbo].[Video] WITH NOCHECK
    ADD CONSTRAINT [FK_Video_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_Wallet_User...';


GO
ALTER TABLE [dbo].[Wallet] WITH NOCHECK
    ADD CONSTRAINT [FK_Wallet_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_WhiteList_User...';


GO
ALTER TABLE [dbo].[WhiteList] WITH NOCHECK
    ADD CONSTRAINT [FK_WhiteList_User] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание FK_WhiteList_User1...';


GO
ALTER TABLE [dbo].[WhiteList] WITH NOCHECK
    ADD CONSTRAINT [FK_WhiteList_User1] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Выполняется создание [dbo].[fn_getFullQualifiedTableName]...';


GO
CREATE FUNCTION [dbo].[fn_getFullQualifiedTableName] (@sql_table_id INT)
RETURNS NVARCHAR(300)
AS
BEGIN
  DECLARE @schema_id BIGINT
  DECLARE @name      NVARCHAR(300)

  SELECT @schema_id = schema_id FROM sys.tables WHERE object_id = @sql_table_id
  SET @name = '[' + SCHEMA_NAME(@schema_id) + '].[' + OBJECT_NAME(@sql_table_id) + ']' 

  RETURN @name
END
GO
PRINT N'Выполняется создание [dbo].[fn_quotename_brackets]...';


GO
CREATE FUNCTION [dbo].[fn_quotename_brackets] (@str_dbname NVARCHAR(258))
RETURNS NVARCHAR(258)
AS
BEGIN
   IF (@str_dbname IS NULL OR LEN(@str_dbname) > 128)
   BEGIN
      DECLARE @msg VARCHAR(128)
      SET @msg = '@str_dbname cannot be NULL nor greater than 128'

--      EXEC('SELECT 1 FROM' + @msg)
--      RAISERROR (@msg,15,10)
      RETURN NULL
   END

   -- If we have [ and ] in the middle of string...remove all of them
   IF (CHARINDEX('[', @str_dbname, 2) > 1 AND
       CHARINDEX(']', @str_dbname, 2) > 1)
   BEGIN
      -- Remove all of them and we will be adding the brackets next
      SET @str_dbname = REPLACE(REPLACE(@str_dbname,'[',''), ']', '')
   END

	-- Return the result of the function
   IF (CHARINDEX('[', @str_dbname) <> 1 OR
       CHARINDEX(']', @str_dbname) <> LEN(@str_dbname))
   BEGIN
      SET @str_dbname = QUOTENAME(@str_dbname)
   END
	RETURN @str_dbname
END
GO
PRINT N'Выполняется создание [dbo].[pr_CreateFkTreeDeleteTrigger]...';


GO
/*
   -- List of Triggers
   SELECT 'DROP TRIGGER [' + SCHEMA_NAME(t.schema_id) + '].[' + tr.name + ']'
     FROM sys.triggers tr
     JOIN sys.objects t on t.object_id = tr.parent_id
*/

--
--  EXEC [acornpa].[pr_FkCreateTreeDeleteTrigger] 1
--
-- Somewhat like a FK Cascade Delete but it is put
-- on the referenced table (aka Parent table) instead of the
-- table with the FK.
CREATE PROCEDURE  [dbo].[pr_CreateFkTreeDeleteTrigger]
       @schema_name NVARCHAR(128) = NULL, -- Null means all Schemas
       @is_verbose BIT = 0
AS
BEGIN
   SET NOCOUNT ON

   DECLARE @cmd             NVARCHAR(MAX)
   DECLARE @table_id        INT
   DECLARE @table_schema    NVARCHAR(128)
   DECLARE @table_name      NVARCHAR(128)
   DECLARE @column_name     NVARCHAR(128)
   DECLARE @column_ordinal  INT

   -- Referenced tables along with their PKs
   DECLARE reftable_cur CURSOR FOR
     SELECT DISTINCT t.object_id, SCHEMA_NAME(t.schema_id) AS table_schema, t.name
       FROM sys.tables t
       JOIN (SELECT DISTINCT fk.referenced_object_id FROM sys.foreign_key_columns fk) ref
         ON t.object_id = ref.referenced_object_id
       JOIN sys.key_constraints kc
         ON kc.parent_object_id = t.object_id
            AND kc.type = 'PK'      
       LEFT JOIN sys.triggers tr
         ON tr.parent_id = t.object_id
            AND tr.is_instead_of_trigger = 1
       LEFT JOIN sys.trigger_events te
         ON te.object_id = tr.object_id
            AND te.type = 3 -- DELETE event
      WHERE t.type = 'U' -- User Defnied Table
        AND te.object_id IS NULL -- We do not have DELETE Trigger on table
		AND (@schema_name IS NULL OR @schema_name = SCHEMA_NAME(t.schema_id)) -- Only in schemas that we were asked
   ORDER BY t.name

   OPEN reftable_cur
   FETCH NEXT FROM reftable_cur INTO @table_id, @table_schema, @table_name
   WHILE @@FETCH_STATUS = 0
   BEGIN
      DECLARE @triggerName      NVARCHAR(128)
      DECLARE @tmpTableName     NVARCHAR(128) 
      DECLARE @list_column_into NVARCHAR(MAX)
      DECLARE @cmd_where_clause NVARCHAR(MAX)

      SET @triggerName = 'TR_TreeDelete_' + @table_name
      SET @tmpTableName = dbo.fn_quotename_brackets('#TMP_DELETED_' + @table_name)
      SET @list_column_into = NULL
      SET @cmd_where_clause = NULL

      DECLARE refcol_cur CURSOR FOR
        SELECT COL_NAME(@table_id, idx_col.column_id) AS column_name, idx_col.key_ordinal
          FROM sys.key_constraints kc
          JOIN sys.indexes idx
            ON kc.unique_index_id = idx.index_id
               AND kc.parent_object_id = idx.object_id
          JOIN sys.index_columns idx_col
            ON idx.object_id = idx_col.object_id
               AND idx.index_id = idx_col.index_id
         WHERE idx_col.is_included_column = 0
           AND kc.parent_object_id = @table_id
           AND kc.type = 'PK'
        ORDER BY idx_col.key_ordinal;

      OPEN refcol_cur
      FETCH NEXT FROM refcol_cur INTO @column_name, @column_ordinal
      WHILE @@FETCH_STATUS = 0
      BEGIN
        IF @list_column_into IS NULL
        BEGIN
           SET @list_column_into = dbo.fn_quotename_brackets(@column_name)
           SET @cmd_where_clause = dbo.fn_quotename_brackets(@table_name)  + '.' + dbo.fn_quotename_brackets(@column_name) + ' IN (SELECT ' + dbo.fn_quotename_brackets(@column_name) + ' FROM ' + @tmpTableName + ')'
        END
        ELSE 
        BEGIN
           SET @list_column_into = @list_column_into + ', ' + dbo.fn_quotename_brackets(@column_name)
           SET @cmd_where_clause = @cmd_where_clause + CHAR(13) + 
               ' AND ' + @table_name  + '.' + dbo.fn_quotename_brackets(@column_name) + ' IN (SELECT ' + dbo.fn_quotename_brackets(@column_name) + ' FROM ' + @tmpTableName + ')'
        END
        FETCH NEXT FROM refcol_cur INTO @column_name, @column_ordinal
      END       
      CLOSE refcol_cur
      DEALLOCATE refcol_cur

      SET @cmd = 
          'CREATE TRIGGER ' + @triggerName + CHAR(13) +
          '    ON ' + dbo.fn_getFullQualifiedTableName(@table_id) + CHAR(13) +
          'INSTEAD OF DELETE AS' + CHAR(13) +
          'BEGIN' + CHAR(13) +
          '  IF @@rowcount = 0 RETURN;' + CHAR(13) +
          '  -- PRINT ''Trigger:' + @triggerName + ' has been called'';' + CHAR(13) +
          '  -- Put DELETED into a temporary table, so it can be seen by stored procedure' + CHAR(13) +
          '  SELECT ' + @list_column_into + ' INTO ' + @tmpTableName + ' FROM DELETED;' + CHAR(13) +
          '  --' + CHAR(13) +
          '  -- Call recursive delete stored procedure' + CHAR(13) +
          '  -- We need to disable the trigger to avoid recursion on an INSTEAD TRIGGER.' + CHAR(13) +
          '  -- This may not be good, if another transaction deletes stuff from the table.' + CHAR(13) +
          '  DISABLE TRIGGER ' +  @triggerName + ' ON ' + dbo.fn_getFullQualifiedTableName(@table_id) + ';' + CHAR(13) +
          '  EXEC dbo.pr_FkTreeDelete' + CHAR(13) +
          '       @parent_table_id = ''' + dbo.fn_getFullQualifiedTableName(@table_id) + ''',' +  CHAR(13) +
          '       @where_clause = ''' + @cmd_where_clause + '''' + CHAR(13) +
          '  ;' + CHAR(13) +
          '  -- Enable it back' + CHAR(13) +
          '  ENABLE TRIGGER ' +  @triggerName + ' ON ' + dbo.fn_getFullQualifiedTableName(@table_id) + ';' + CHAR(13) +
          '  DROP TABLE ' + @tmpTableName + ';' + CHAR(13) +
          'END' + CHAR(13)

      IF (@is_verbose = 1) PRINT @cmd

      EXEC(@cmd)
      FETCH NEXT FROM reftable_cur INTO @table_id, @table_schema, @table_name
   END       
   CLOSE reftable_cur
   DEALLOCATE reftable_cur
END
GO
PRINT N'Выполняется создание [dbo].[pr_FkTreeDelete]...';


GO
--
-- Delete all records in the table specified that conform to the given 
-- where clause while also deleting children, grandchildren, and so on
-- in a fashion like a recursive delete (Cascading Delete) based on
-- the foreign keys.
--
-- This stored procedure is handy when the tables do not have the cascade
-- delete option.
-- Sometimes CASCATE DELETE option in SQL is not possible due to SQL
-- restrictions:
--    1) There can be only one cascade path from a parent table to a child
--       table. That is, no table can appear more than once in the list of all
--       cascading referential actions.
--    2) Related to above rule, a Self-referencing constraints will not allow
--       CASCADE operations to be defined on them. A classic example is the
--       EMPLOYEE table that has a MANAGER column would exhibit this issue.
--    What SQL Server indicates when the above rules are not match, is 
--    something like CASCATE does must form a tree that contains no circular
--    references.
--
--    http://msdn.microsoft.com/en-us/library/ms186973.aspx
--
-- The idea for this script was taken from Daniel Crowther, who wrote a script
-- to emulate cascading deletes in SQL Server 7. I have very much rewritten 
-- the script from scratch. Just the idea has been taken.
--
-- Example 1
--    EXEC acornpa.pr_fkTreeDelete
--         @parent_table_id = 'shiner.Scenarios',
--         @where_clause = 'Scenarios.Id = 123'
--
--
--
CREATE PROCEDURE [dbo].[pr_FkTreeDelete]
   @parent_table_id NVARCHAR(300),       -- SQL Table_id or SQL TABLE NAME (e.g., shiner.Entities) where rows are to be deleted
   @where_clause    NVARCHAR(MAX),       -- WHERE CLAUSE (Entities.Id = 7) Usually table.pkey = value used to delete records
   @from_clause     NVARCHAR(MAX) = '',  -- ONLY For internal purposes (aka Recursion). Use default
   @cascate_level   INT = 0,             -- ONLY For internal purposes (aka Recursion). Use default
   @is_verbose      BIT = 0
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @do_exec  BIT     SET @do_exec = 1 
  DECLARE @NEW_LINE CHAR(1) SET @NEW_LINE = CHAR(13) 

  DECLARE @cmd             NVARCHAR(MAX)
  DECLARE @cmd_from        NVARCHAR(1000)
  DECLARE @cmd_onjoin      NVARCHAR(MAX)
  DECLARE @fk_const_id     INT
  DECLARE @fk_table_id     INT
  DECLARE @child_level     INT

  IF ISNUMERIC(@parent_table_id) = 0
  BEGIN 
     -- Get the table identifier
     IF CHARINDEX('.', @parent_table_id) = 0
       SET @parent_table_id = OBJECT_ID('[dbo].[' + @parent_table_id + ']')
     ELSE
       SET @parent_table_id = OBJECT_ID(@parent_table_id)
  END

  IF @cascate_level = 0
  BEGIN
     SET @cmd = 'SET NOCOUNT ON' + @NEW_LINE
     SET @from_clause = 'FROM ' + dbo.fn_getFullQualifiedTableName(@parent_table_id)

     IF (@is_verbose = 1) 
     BEGIN
       PRINT '-- **************************************************************'
       PRINT '-- *** Cascade delete for table' + dbo.fn_getFullQualifiedTableName(@parent_table_id)
       PRINT '-- *** with where_clause of ' + @where_clause
       PRINT '-- **************************************************************'
       PRINT @cmd
     END
     IF (@do_exec = 1) EXEC (@cmd) 
  END

  -- Referenced_object_id is the table with the PK 
  -- Parent_object_id is the table with the FK
  DECLARE children_cur CURSOR LOCAL FORWARD_ONLY FOR
   SELECT DISTINCT
          fkNameId = fk.object_id,          -- FK constraint id
          fkTableId = fk.parent_object_id   -- The parent of the constraint, which is the referencing table. The table with the FK (aka child table)
  --      refTableId = referenced_object_id -- The table that we refer to (aka parent table)
     FROM sys.foreign_keys fk 
    WHERE fk.referenced_object_id <> fk.parent_object_id -- WE DO NOT HANDLE self referencing tables!!!
      AND fk.referenced_object_id = @parent_table_id
      AND fk.delete_referential_action <> 1 -- MAKE sure it is not ON DELETE CASCATE in the FK. If it is SQL will do it

  OPEN children_cur 
  FETCH NEXT FROM children_cur INTO @fk_const_id, @fk_table_id
  WHILE @@FETCH_STATUS = 0
  BEGIN
    -- Build a join criteria to delete rows from child table.
    -- The JOINS keep growing with each recursive call
    SET @cmd_onjoin = 
        STUFF((SELECT ' AND' + @NEW_LINE +
                      '[' + OBJECT_NAME(rkeyid) + '].[' + COL_NAME(rkeyid, rkey) + '] = ' +
                      '[' + OBJECT_NAME(fkeyid) + '].[' + COL_NAME(fkeyid, fkey) + ']' 'text()'
                 FROM dbo.sysforeignkeys fk
                WHERE fk.constid = @fk_const_id 
               FOR XML PATH(''), TYPE).value('text()[1]', 'VARCHAR(MAX)')
             , 1, LEN(' AND' + @NEW_LINE), '')
    SELECT @cmd_from = 
           @from_clause + @NEW_LINE +
           'JOIN ' + dbo.fn_getFullQualifiedTableName(@fk_table_id) + @NEW_LINE +
           '  ON ' + @cmd_onjoin

    SET @child_level = @cascate_level + 1
    EXEC dbo.pr_FkTreeDelete
         @parent_table_id = @fk_table_id,
         @where_clause = @where_clause,
         @from_clause = @cmd_from,
         @cascate_level = @child_level,
         @is_verbose = @is_verbose 

    -- NOTE: When a DELETE action to a child/referencing table is the result of 
    --       a CASCADE on a DELTE from parent table, and an INSTEAD of trigger
    --       on DELETE is defined on that child table, the trigger is IGNORED
    --       and the DELETE action is executed. THUS, you would not have 
    --       called the stuff twice.
    --       See Remarks on 
    --           http://msdn.microsoft.com/en-us/library/ms176072.aspx
	 SET @cmd =  
        'DELETE FROM ' + dbo.fn_getFullQualifiedTableName(@fk_table_id) + ' -- Level=' + CAST(@cascate_level AS NVARCHAR) + @NEW_LINE +
        @cmd_from + @NEW_LINE +
        'WHERE ' + @where_clause + @NEW_LINE

    IF (@is_verbose = 1) PRINT @cmd
    IF (@do_exec = 1) EXEC (@cmd) 

	 FETCH NEXT FROM children_cur INTO @fk_const_id, @fk_table_id
  END

  IF @cascate_level = 0
  BEGIN
    -- Delete the rows from parent table
    SET @cmd = 
        'DELETE FROM ' + dbo.fn_getFullQualifiedTableName(@parent_table_id) + ' -- TOP LEVEL PARENT TABLE' + @NEW_LINE +
        ' WHERE ' + @where_clause + @NEW_LINE

    IF (@is_verbose = 1) 
    BEGIN
       PRINT @cmd
    END

    IF (@do_exec = 1) 
    BEGIN
      EXEC (@cmd) 
      PRINT 'Deleted ' + CONVERT(varchar, @@ROWCOUNT) + ' records from table ' + dbo.fn_getFullQualifiedTableName(@parent_table_id)
    END
  END

  CLOSE children_cur 
  DEALLOCATE children_cur 
END
GO
PRINT N'Выполняется создание [dbo].[pr_PopulateStaticData]...';


GO
CREATE PROCEDURE [dbo].[pr_PopulateStaticData]
AS
BEGIN
	DELETE FROM [dbo].[Merchants]

	INSERT INTO [dbo].[Merchants]
	SELECT 'PayPal'

	DELETE FROM [dbo].[Roles]

	INSERT INTO [dbo].[Roles]
	SELECT 'Administrator'
	UNION ALL
	SELECT 'Helper'
	UNION ALL
	SELECT 'Performer'
	UNION ALL
	SELECT 'Advertiser'
	UNION ALL
	SELECT 'Banned'

	DELETE FROM [dbo].[TicketThemes]

	INSERT INTO [dbo].[TicketThemes]
	SELECT 'Wallet'
	UNION ALL
	SELECT 'Orders'
	UNION ALL
	SELECT 'Platform'
	UNION ALL
	SELECT 'Messages'
	UNION ALL
	SELECT 'Other'

	DELETE FROM [dbo].[Regions]

	INSERT INTO [dbo].[Regions]
	SELECT 'Any'
	UNION ALL
	SELECT 'Eastern Africa'
	UNION ALL
	SELECT 'Middle Africa'
	UNION ALL
	SELECT 'Northern Africa'
	UNION ALL
	SELECT 'Southern Africa'
	UNION ALL
	SELECT 'Western Africa'
	UNION ALL
	SELECT 'Latin America'
	UNION ALL
	SELECT 'Northern America'
	UNION ALL
	SELECT 'Antarctica'
	UNION ALL
	SELECT 'Central Asia'
	UNION ALL
	SELECT 'Eastern Asia'
	UNION ALL
	SELECT 'Southern Asia'
	UNION ALL
	SELECT 'South-Eastern Asia'
	UNION ALL
	SELECT 'Western Asia'
	UNION ALL
	SELECT 'Eastern Europe'
	UNION ALL
	SELECT 'Northern Europe'
	UNION ALL
	SELECT 'Southern Europe'
	UNION ALL
	SELECT 'Western Europe'
	UNION ALL
	SELECT 'Australia and New Zealand'
	UNION ALL
	SELECT 'Melanesia'
	UNION ALL
	SELECT 'Micronesia'
	UNION ALL
	SELECT 'Polynesia'

	DELETE FROM [dbo].[Timezone]

	INSERT INTO [dbo].[Timezone]
	SELECT '-12'
	UNION ALL
	SELECT '-11'
	UNION ALL
	SELECT '-10'
	UNION ALL
	SELECT '-9:30'
	UNION ALL
	SELECT '-9'
	UNION ALL
	SELECT '-8:30'
	UNION ALL
	SELECT '-7'
	UNION ALL
	SELECT '-6'
	UNION ALL
	SELECT '-5'
	UNION ALL
	SELECT '-4:30'
	UNION ALL
	SELECT '-4'
	UNION ALL
	SELECT '-3:30'
	UNION ALL
	SELECT '-3'
	UNION ALL
	SELECT '-2:30'
	UNION ALL
	SELECT '-2'
	UNION ALL
	SELECT '-1'
	UNION ALL
	SELECT '-0:44'
	UNION ALL
	SELECT '-0:25'
	UNION ALL
	SELECT 'UTC 0:00'
	UNION ALL
	SELECT '+0:20'
	UNION ALL
	SELECT '+0:30'
	UNION ALL
	SELECT '+1'
	UNION ALL
	SELECT '+2'
	UNION ALL
	SELECT '+3'
	UNION ALL
	SELECT '+3:30'
	UNION ALL
	SELECT '+4'
	UNION ALL
	SELECT '+4:30'
	UNION ALL
	SELECT '+4:51'
	UNION ALL
	SELECT '+5'
	UNION ALL
	SELECT '+5:30'
	UNION ALL
	SELECT '+5:40'
	UNION ALL
	SELECT '+5:45'
	UNION ALL
	SELECT '6'
	UNION ALL
	SELECT '+6:30'
	UNION ALL
	SELECT '+7'
	UNION ALL
	SELECT '+7:30'
	UNION ALL
	SELECT '+8'
	UNION ALL
	SELECT '+8:45'
	UNION ALL
	SELECT '+9'
	UNION ALL
	SELECT '+9:30'
	UNION ALL
	SELECT '+10'
	UNION ALL
	SELECT '+10:30'
	UNION ALL
	SELECT '+11'
	UNION ALL
	SELECT '+11:30'
	UNION ALL
	SELECT '+12'
	UNION ALL
	SELECT '+12:45'
	UNION ALL
	SELECT '+13'
	UNION ALL
	SELECT '+13:45'
	UNION ALL
	SELECT '+14'

	DELETE FROM [dbo].[Themes]

	INSERT INTO [dbo].[Themes]
	SELECT 'Popular'
	UNION ALL
	SELECT 'News'
	UNION ALL
	SELECT 'Entertainment'
	UNION ALL
	SELECT 'Sport'
	UNION ALL
	SELECT 'Animals & Wildlife'
	UNION ALL
	SELECT 'Music'
	UNION ALL
	SELECT 'Technology'
	UNION ALL
	SELECT 'Gaming'
	UNION ALL
	SELECT 'Education'

	DELETE FROM [dbo].[Geolocation]

	INSERT INTO [dbo].[Geolocation] 
	(
		[ISO2],
		[CountryName],
		[LongCountryName],
		[ISO3],
		[NumCode],
		[UNMemberState],
		[CallingCode],
		[CCTLD]
	)
	VALUES
	('AF','Afghanistan','Islamic Republic of Afghanistan','AFG','004','yes','93','.af'),
	('AX','Aland Islands','&Aring;land Islands','ALA','248','no','358','.ax'),
	('AL','Albania','Republic of Albania','ALB','008','yes','355','.al'),
	('DZ','Algeria','People''s Democratic Republic of Algeria','DZA','012','yes','213','.dz'),
	('AS','American Samoa','American Samoa','ASM','016','no','1+684','.as'),
	('AD','Andorra','Principality of Andorra','AND','020','yes','376','.ad'),
	('AO','Angola','Republic of Angola','AGO','024','yes','244','.ao'),
	('AI','Anguilla','Anguilla','AIA','660','no','1+264','.ai'),
	('AQ','Antarctica','Antarctica','ATA','010','no','672','.aq'),
	('AG','Antigua and Barbuda','Antigua and Barbuda','ATG','028','yes','1+268','.ag'),
	('AR','Argentina','Argentine Republic','ARG','032','yes','54','.ar'),
	('AM','Armenia','Republic of Armenia','ARM','051','yes','374','.am'),
	('AW','Aruba','Aruba','ABW','533','no','297','.aw'),
	('AU','Australia','Commonwealth of Australia','AUS','036','yes','61','.au'),
	('AT','Austria','Republic of Austria','AUT','040','yes','43','.at'),
	('AZ','Azerbaijan','Republic of Azerbaijan','AZE','031','yes','994','.az'),
	('BS','Bahamas','Commonwealth of The Bahamas','BHS','044','yes','1+242','.bs'),
	('BH','Bahrain','Kingdom of Bahrain','BHR','048','yes','973','.bh'),
	('BD','Bangladesh','People''s Republic of Bangladesh','BGD','050','yes','880','.bd'),
	('BB','Barbados','Barbados','BRB','052','yes','1+246','.bb'),
	('BY','Belarus','Republic of Belarus','BLR','112','yes','375','.by'),
	('BE','Belgium','Kingdom of Belgium','BEL','056','yes','32','.be'),
	('BZ','Belize','Belize','BLZ','084','yes','501','.bz'),
	('BJ','Benin','Republic of Benin','BEN','204','yes','229','.bj'),
	('BM','Bermuda','Bermuda Islands','BMU','060','no','1+441','.bm'),
	('BT','Bhutan','Kingdom of Bhutan','BTN','064','yes','975','.bt'),
	('BO','Bolivia','Plurinational State of Bolivia','BOL','068','yes','591','.bo'),
	('BQ','Bonaire, Sint Eustatius and Saba','Bonaire, Sint Eustatius and Saba','BES','535','no','599','.bq'),
	('BA','Bosnia and Herzegovina','Bosnia and Herzegovina','BIH','070','yes','387','.ba'),
	('BW','Botswana','Republic of Botswana','BWA','072','yes','267','.bw'),
	('BV','Bouvet Island','Bouvet Island','BVT','074','no','NONE','.bv'),
	('BR','Brazil','Federative Republic of Brazil','BRA','076','yes','55','.br'),
	('IO','British Indian Ocean Territory','British Indian Ocean Territory','IOT','086','no','246','.io'),
	('BN','Brunei','Brunei Darussalam','BRN','096','yes','673','.bn'),
	('BG','Bulgaria','Republic of Bulgaria','BGR','100','yes','359','.bg'),
	('BF','Burkina Faso','Burkina Faso','BFA','854','yes','226','.bf'),
	('BI','Burundi','Republic of Burundi','BDI','108','yes','257','.bi'),
	('KH','Cambodia','Kingdom of Cambodia','KHM','116','yes','855','.kh'),
	('CM','Cameroon','Republic of Cameroon','CMR','120','yes','237','.cm'),
	('CA','Canada','Canada','CAN','124','yes','1','.ca'),
	('CV','Cape Verde','Republic of Cape Verde','CPV','132','yes','238','.cv'),
	('KY','Cayman Islands','The Cayman Islands','CYM','136','no','1+345','.ky'),
	('CF','Central African Republic','Central African Republic','CAF','140','yes','236','.cf'),
	('TD','Chad','Republic of Chad','TCD','148','yes','235','.td'),
	('CL','Chile','Republic of Chile','CHL','152','yes','56','.cl'),
	('CN','China','People''s Republic of China','CHN','156','yes','86','.cn'),
	('CX','Christmas Island','Christmas Island','CXR','162','no','61','.cx'),
	('CC','Cocos (Keeling) Islands','Cocos (Keeling) Islands','CCK','166','no','61','.cc'),
	('CO','Colombia','Republic of Colombia','COL','170','yes','57','.co'),
	('KM','Comoros','Union of the Comoros','COM','174','yes','269','.km'),
	('CG','Congo','Republic of the Congo','COG','178','yes','242','.cg'),
	('CK','Cook Islands','Cook Islands','COK','184','some','682','.ck'),
	('CR','Costa Rica','Republic of Costa Rica','CRI','188','yes','506','.cr'),
	('CI','Cote d''ivoire (Ivory Coast)','Republic of C&ocirc;te D''Ivoire (Ivory Coast)','CIV','384','yes','225','.ci'),
	('HR','Croatia','Republic of Croatia','HRV','191','yes','385','.hr'),
	('CU','Cuba','Republic of Cuba','CUB','192','yes','53','.cu'),
	('CW','Curacao','Cura&ccedil;ao','CUW','531','no','599','.cw'),
	('CY','Cyprus','Republic of Cyprus','CYP','196','yes','357','.cy'),
	('CZ','Czech Republic','Czech Republic','CZE','203','yes','420','.cz'),
	('CD','Democratic Republic of the Congo','Democratic Republic of the Congo','COD','180','yes','243','.cd'),
	('DK','Denmark','Kingdom of Denmark','DNK','208','yes','45','.dk'),
	('DJ','Djibouti','Republic of Djibouti','DJI','262','yes','253','.dj'),
	('DM','Dominica','Commonwealth of Dominica','DMA','212','yes','1+767','.dm'),
	('DO','Dominican Republic','Dominican Republic','DOM','214','yes','1+809, 8','.do'),
	('EC','Ecuador','Republic of Ecuador','ECU','218','yes','593','.ec'),
	('EG','Egypt','Arab Republic of Egypt','EGY','818','yes','20','.eg'),
	('SV','El Salvador','Republic of El Salvador','SLV','222','yes','503','.sv'),
	('GQ','Equatorial Guinea','Republic of Equatorial Guinea','GNQ','226','yes','240','.gq'),
	('ER','Eritrea','State of Eritrea','ERI','232','yes','291','.er'),
	('EE','Estonia','Republic of Estonia','EST','233','yes','372','.ee'),
	('ET','Ethiopia','Federal Democratic Republic of Ethiopia','ETH','231','yes','251','.et'),
	('FK','Falkland Islands (Malvinas)','The Falkland Islands (Malvinas)','FLK','238','no','500','.fk'),
	('FO','Faroe Islands','The Faroe Islands','FRO','234','no','298','.fo'),
	('FJ','Fiji','Republic of Fiji','FJI','242','yes','679','.fj'),
	('FI','Finland','Republic of Finland','FIN','246','yes','358','.fi'),
	('FR','France','French Republic','FRA','250','yes','33','.fr'),
	('GF','French Guiana','French Guiana','GUF','254','no','594','.gf'),
	('PF','French Polynesia','French Polynesia','PYF','258','no','689','.pf'),
	('TF','French Southern Territories','French Southern Territories','ATF','260','no','NULL','.tf'),
	('GA','Gabon','Gabonese Republic','GAB','266','yes','241','.ga'),
	('GM','Gambia','Republic of The Gambia','GMB','270','yes','220','.gm'),
	('GE','Georgia','Georgia','GEO','268','yes','995','.ge'),
	('DE','Germany','Federal Republic of Germany','DEU','276','yes','49','.de'),
	('GH','Ghana','Republic of Ghana','GHA','288','yes','233','.gh'),
	('GI','Gibraltar','Gibraltar','GIB','292','no','350','.gi'),
	('GR','Greece','Hellenic Republic','GRC','300','yes','30','.gr'),
	('GL','Greenland','Greenland','GRL','304','no','299','.gl'),
	('GD','Grenada','Grenada','GRD','308','yes','1+473','.gd'),
	('GP','Guadaloupe','Guadeloupe','GLP','312','no','590','.gp'),
	('GU','Guam','Guam','GUM','316','no','1+671','.gu'),
	('GT','Guatemala','Republic of Guatemala','GTM','320','yes','502','.gt'),
	('GG','Guernsey','Guernsey','GGY','831','no','44','.gg'),
	('GN','Guinea','Republic of Guinea','GIN','324','yes','224','.gn'),
	('GW','Guinea-Bissau','Republic of Guinea-Bissau','GNB','624','yes','245','.gw'),
	('GY','Guyana','Co-operative Republic of Guyana','GUY','328','yes','592','.gy'),
	('HT','Haiti','Republic of Haiti','HTI','332','yes','509','.ht'),
	('HM','Heard Island and McDonald Islands','Heard Island and McDonald Islands','HMD','334','no','NONE','.hm'),
	('HN','Honduras','Republic of Honduras','HND','340','yes','504','.hn'),
	('HK','Hong Kong','Hong Kong','HKG','344','no','852','.hk'),
	('HU','Hungary','Hungary','HUN','348','yes','36','.hu'),
	('IS','Iceland','Republic of Iceland','ISL','352','yes','354','.is'),
	('IN','India','Republic of India','IND','356','yes','91','.in'),
	('ID','Indonesia','Republic of Indonesia','IDN','360','yes','62','.id'),
	('IR','Iran','Islamic Republic of Iran','IRN','364','yes','98','.ir'),
	('IQ','Iraq','Republic of Iraq','IRQ','368','yes','964','.iq'),
	('IE','Ireland','Ireland','IRL','372','yes','353','.ie'),
	('IM','Isle of Man','Isle of Man','IMN','833','no','44','.im'),
	('IL','Israel','State of Israel','ISR','376','yes','972','.il'),
	('IT','Italy','Italian Republic','ITA','380','yes','39','.jm'),
	('JM','Jamaica','Jamaica','JAM','388','yes','1+876','.jm'),
	('JP','Japan','Japan','JPN','392','yes','81','.jp'),
	('JE','Jersey','The Bailiwick of Jersey','JEY','832','no','44','.je'),
	('JO','Jordan','Hashemite Kingdom of Jordan','JOR','400','yes','962','.jo'),
	('KZ','Kazakhstan','Republic of Kazakhstan','KAZ','398','yes','7','.kz'),
	('KE','Kenya','Republic of Kenya','KEN','404','yes','254','.ke'),
	('KI','Kiribati','Republic of Kiribati','KIR','296','yes','686','.ki'),
	('XK','Kosovo','Republic of Kosovo','---','---','some','381',''),
	('KW','Kuwait','State of Kuwait','KWT','414','yes','965','.kw'),
	('KG','Kyrgyzstan','Kyrgyz Republic','KGZ','417','yes','996','.kg'),
	('LA','Laos','Lao People''s Democratic Republic','LAO','418','yes','856','.la'),
	('LV','Latvia','Republic of Latvia','LVA','428','yes','371','.lv'),
	('LB','Lebanon','Republic of Lebanon','LBN','422','yes','961','.lb'),
	('LS','Lesotho','Kingdom of Lesotho','LSO','426','yes','266','.ls'),
	('LR','Liberia','Republic of Liberia','LBR','430','yes','231','.lr'),
	('LY','Libya','Libya','LBY','434','yes','218','.ly'),
	('LI','Liechtenstein','Principality of Liechtenstein','LIE','438','yes','423','.li'),
	('LT','Lithuania','Republic of Lithuania','LTU','440','yes','370','.lt'),
	('LU','Luxembourg','Grand Duchy of Luxembourg','LUX','442','yes','352','.lu'),
	('MO','Macao','The Macao Special Administrative Region','MAC','446','no','853','.mo'),
	('MK','Macedonia','The Former Yugoslav Republic of Macedonia','MKD','807','yes','389','.mk'),
	('MG','Madagascar','Republic of Madagascar','MDG','450','yes','261','.mg'),
	('MW','Malawi','Republic of Malawi','MWI','454','yes','265','.mw'),
	('MY','Malaysia','Malaysia','MYS','458','yes','60','.my'),
	('MV','Maldives','Republic of Maldives','MDV','462','yes','960','.mv'),
	('ML','Mali','Republic of Mali','MLI','466','yes','223','.ml'),
	('MT','Malta','Republic of Malta','MLT','470','yes','356','.mt'),
	('MH','Marshall Islands','Republic of the Marshall Islands','MHL','584','yes','692','.mh'),
	('MQ','Martinique','Martinique','MTQ','474','no','596','.mq'),
	('MR','Mauritania','Islamic Republic of Mauritania','MRT','478','yes','222','.mr'),
	('MU','Mauritius','Republic of Mauritius','MUS','480','yes','230','.mu'),
	('YT','Mayotte','Mayotte','MYT','175','no','262','.yt'),
	('MX','Mexico','United Mexican States','MEX','484','yes','52','.mx'),
	('FM','Micronesia','Federated States of Micronesia','FSM','583','yes','691','.fm'),
	('MD','Moldava','Republic of Moldova','MDA','498','yes','373','.md'),
	('MC','Monaco','Principality of Monaco','MCO','492','yes','377','.mc'),
	('MN','Mongolia','Mongolia','MNG','496','yes','976','.mn'),
	('ME','Montenegro','Montenegro','MNE','499','yes','382','.me'),
	('MS','Montserrat','Montserrat','MSR','500','no','1+664','.ms'),
	('MA','Morocco','Kingdom of Morocco','MAR','504','yes','212','.ma'),
	('MZ','Mozambique','Republic of Mozambique','MOZ','508','yes','258','.mz'),
	('MM','Myanmar (Burma)','Republic of the Union of Myanmar','MMR','104','yes','95','.mm'),
	('NA','Namibia','Republic of Namibia','NAM','516','yes','264','.na'),
	('NR','Nauru','Republic of Nauru','NRU','520','yes','674','.nr'),
	('NP','Nepal','Federal Democratic Republic of Nepal','NPL','524','yes','977','.np'),
	('NL','Netherlands','Kingdom of the Netherlands','NLD','528','yes','31','.nl'),
	('NC','New Caledonia','New Caledonia','NCL','540','no','687','.nc'),
	('NZ','New Zealand','New Zealand','NZL','554','yes','64','.nz'),
	('NI','Nicaragua','Republic of Nicaragua','NIC','558','yes','505','.ni'),
	('NE','Niger','Republic of Niger','NER','562','yes','227','.ne'),
	('NG','Nigeria','Federal Republic of Nigeria','NGA','566','yes','234','.ng'),
	('NU','Niue','Niue','NIU','570','some','683','.nu'),
	('NF','Norfolk Island','Norfolk Island','NFK','574','no','672','.nf'),
	('KP','North Korea','Democratic People''s Republic of Korea','PRK','408','yes','850','.kp'),
	('MP','Northern Mariana Islands','Northern Mariana Islands','MNP','580','no','1+670','.mp'),
	('NO','Norway','Kingdom of Norway','NOR','578','yes','47','.no'),
	('OM','Oman','Sultanate of Oman','OMN','512','yes','968','.om'),
	('PK','Pakistan','Islamic Republic of Pakistan','PAK','586','yes','92','.pk'),
	('PW','Palau','Republic of Palau','PLW','585','yes','680','.pw'),
	('PS','Palestine','State of Palestine (or Occupied Palestinian Territory)','PSE','275','some','970','.ps'),
	('PA','Panama','Republic of Panama','PAN','591','yes','507','.pa'),
	('PG','Papua New Guinea','Independent State of Papua New Guinea','PNG','598','yes','675','.pg'),
	('PY','Paraguay','Republic of Paraguay','PRY','600','yes','595','.py'),
	('PE','Peru','Republic of Peru','PER','604','yes','51','.pe'),
	('PH','Phillipines','Republic of the Philippines','PHL','608','yes','63','.ph'),
	('PN','Pitcairn','Pitcairn','PCN','612','no','NONE','.pn'),
	('PL','Poland','Republic of Poland','POL','616','yes','48','.pl'),
	('PT','Portugal','Portuguese Republic','PRT','620','yes','351','.pt'),
	('PR','Puerto Rico','Commonwealth of Puerto Rico','PRI','630','no','1+939','.pr'),
	('QA','Qatar','State of Qatar','QAT','634','yes','974','.qa'),
	('RE','Reunion','R&eacute;union','REU','638','no','262','.re'),
	('RO','Romania','Romania','ROU','642','yes','40','.ro'),
	('RU','Russia','Russian Federation','RUS','643','yes','7','.ru'),
	('RW','Rwanda','Republic of Rwanda','RWA','646','yes','250','.rw'),
	('BL','Saint Barthelemy','Saint Barth&eacute;lemy','BLM','652','no','590','.bl'),
	('SH','Saint Helena','Saint Helena, Ascension and Tristan da Cunha','SHN','654','no','290','.sh'),
	('KN','Saint Kitts and Nevis','Federation of Saint Christopher and Nevis','KNA','659','yes','1+869','.kn'),
	('LC','Saint Lucia','Saint Lucia','LCA','662','yes','1+758','.lc'),
	('MF','Saint Martin','Saint Martin','MAF','663','no','590','.mf'),
	('PM','Saint Pierre and Miquelon','Saint Pierre and Miquelon','SPM','666','no','508','.pm'),
	('VC','Saint Vincent and the Grenadines','Saint Vincent and the Grenadines','VCT','670','yes','1+784','.vc'),
	('WS','Samoa','Independent State of Samoa','WSM','882','yes','685','.ws'),
	('SM','San Marino','Republic of San Marino','SMR','674','yes','378','.sm'),
	('ST','Sao Tome and Principe','Democratic Republic of S&atilde;o Tom&eacute; and Pr&iacute;ncipe','STP','678','yes','239','.st'),
	('SA','Saudi Arabia','Kingdom of Saudi Arabia','SAU','682','yes','966','.sa'),
	('SN','Senegal','Republic of Senegal','SEN','686','yes','221','.sn'),
	('RS','Serbia','Republic of Serbia','SRB','688','yes','381','.rs'),
	('SC','Seychelles','Republic of Seychelles','SYC','690','yes','248','.sc'),
	('SL','Sierra Leone','Republic of Sierra Leone','SLE','694','yes','232','.sl'),
	('SG','Singapore','Republic of Singapore','SGP','702','yes','65','.sg'),
	('SX','Sint Maarten','Sint Maarten','SXM','534','no','1+721','.sx'),
	('SK','Slovakia','Slovak Republic','SVK','703','yes','421','.sk'),
	('SI','Slovenia','Republic of Slovenia','SVN','705','yes','386','.si'),
	('SB','Solomon Islands','Solomon Islands','SLB','090','yes','677','.sb'),
	('SO','Somalia','Somali Republic','SOM','706','yes','252','.so'),
	('ZA','South Africa','Republic of South Africa','ZAF','710','yes','27','.za'),
	('GS','South Georgia and the South Sandwich Islands','South Georgia and the South Sandwich Islands','SGS','239','no','500','.gs'),
	('KR','South Korea','Republic of Korea','KOR','410','yes','82','.kr'),
	('SS','South Sudan','Republic of South Sudan','SSD','728','yes','211','.ss'),
	('ES','Spain','Kingdom of Spain','ESP','724','yes','34','.es'),
	('LK','Sri Lanka','Democratic Socialist Republic of Sri Lanka','LKA','144','yes','94','.lk'),
	('SD','Sudan','Republic of the Sudan','SDN','729','yes','249','.sd'),
	('SR','Suriname','Republic of Suriname','SUR','740','yes','597','.sr'),
	('SJ','Svalbard and Jan Mayen','Svalbard and Jan Mayen','SJM','744','no','47','.sj'),
	('SZ','Swaziland','Kingdom of Swaziland','SWZ','748','yes','268','.sz'),
	('SE','Sweden','Kingdom of Sweden','SWE','752','yes','46','.se'),
	('CH','Switzerland','Swiss Confederation','CHE','756','yes','41','.ch'),
	('SY','Syria','Syrian Arab Republic','SYR','760','yes','963','.sy'),
	('TW','Taiwan','Republic of China (Taiwan)','TWN','158','former','886','.tw'),
	('TJ','Tajikistan','Republic of Tajikistan','TJK','762','yes','992','.tj'),
	('TZ','Tanzania','United Republic of Tanzania','TZA','834','yes','255','.tz'),
	('TH','Thailand','Kingdom of Thailand','THA','764','yes','66','.th'),
	('TL','Timor-Leste (East Timor)','Democratic Republic of Timor-Leste','TLS','626','yes','670','.tl'),
	('TG','Togo','Togolese Republic','TGO','768','yes','228','.tg'),
	('TK','Tokelau','Tokelau','TKL','772','no','690','.tk'),
	('TO','Tonga','Kingdom of Tonga','TON','776','yes','676','.to'),
	('TT','Trinidad and Tobago','Republic of Trinidad and Tobago','TTO','780','yes','1+868','.tt'),
	('TN','Tunisia','Republic of Tunisia','TUN','788','yes','216','.tn'),
	('TR','Turkey','Republic of Turkey','TUR','792','yes','90','.tr'),
	('TM','Turkmenistan','Turkmenistan','TKM','795','yes','993','.tm'),
	('TC','Turks and Caicos Islands','Turks and Caicos Islands','TCA','796','no','1+649','.tc'),
	('TV','Tuvalu','Tuvalu','TUV','798','yes','688','.tv'),
	('UG','Uganda','Republic of Uganda','UGA','800','yes','256','.ug'),
	('UA','Ukraine','Ukraine','UKR','804','yes','380','.ua'),
	('AE','United Arab Emirates','United Arab Emirates','ARE','784','yes','971','.ae'),
	('GB','United Kingdom','United Kingdom of Great Britain and Nothern Ireland','GBR','826','yes','44','.uk'),
	('US','United States','United States of America','USA','840','yes','1','.us'),
	('UM','United States Minor Outlying Islands','United States Minor Outlying Islands','UMI','581','no','NONE','NONE'),
	('UY','Uruguay','Eastern Republic of Uruguay','URY','858','yes','598','.uy'),
	('UZ','Uzbekistan','Republic of Uzbekistan','UZB','860','yes','998','.uz'),
	('VU','Vanuatu','Republic of Vanuatu','VUT','548','yes','678','.vu'),
	('VA','Vatican City','State of the Vatican City','VAT','336','no','39','.va'),
	('VE','Venezuela','Bolivarian Republic of Venezuela','VEN','862','yes','58','.ve'),
	('VN','Vietnam','Socialist Republic of Vietnam','VNM','704','yes','84','.vn'),
	('VG','Virgin Islands, British','British Virgin Islands','VGB','092','no','1+284','.vg'),
	('VI','Virgin Islands, US','Virgin Islands of the United States','VIR','850','no','1+340','.vi'),
	('WF','Wallis and Futuna','Wallis and Futuna','WLF','876','no','681','.wf'),
	('EH','Western Sahara','Western Sahara','ESH','732','no','212','.eh'),
	('YE','Yemen','Republic of Yemen','YEM','887','yes','967','.ye'),
	('ZM','Zambia','Republic of Zambia','ZMB','894','yes','260','.zm'),
	('ZW','Zimbabwe','Republic of Zimbabwe','ZWE','716','yes','263','.zw')

	DELETE FROM [dbo].[Certificates]

	INSERT INTO [dbo].[Certificates] 
	(
		[UserId],
		[Code],
		[Status]
	)
	VALUES
	(NULL, 'UHK9yFhgWvqAQjUFNzwZSFnO', 'waiting'),
	(NULL, 'OY1Cu82A3RblzAmhEBZtpPC9', 'waiting'),
	(NULL, '2cCdpzmpf2xSnqGs8dAy9oAn', 'waiting'),
	(NULL, 'XXmjq86xYNEmaoXxOR9limaA', 'waiting'),
	(NULL, 'tkCieZuSUepNBYMlOuF1vEOX', 'waiting'),
	(NULL, 'suTx20EfphKVTz9NPSBrSviL', 'waiting'),
	(NULL, 'XopCVa7vCQ9qSqFJNPyADJxf', 'waiting'),
	(NULL, 'w6widn7E01yDiPFtfeTfloNV', 'waiting'),
	(NULL, 'HvKBbmXx6gkV1kPnmUo3CF3H', 'waiting'),
	(NULL, '71z5ugNnsaklZylrhBvZisWe', 'waiting'),
	(NULL, '9p01SOynbB8BNNv9JZ4azCet', 'waiting'),
	(NULL, '9Mj9S00qXavsEVkyxk3unVfp', 'waiting'),
	(NULL, 'Mwr00VqwzYxL3q7we2FEkYV9', 'waiting'),
	(NULL, 'ergvzi22S9iEQaW31AQSW2Kv', 'waiting'),
	(NULL, 'OrRvMBO4MViXroQelccdOKSL', 'waiting'),
	(NULL, '9mjHXkheO8v5px5MNV55H6Sj', 'waiting'),
	(NULL, '2d2Ng3EHgklIOkRCpK2N90oF', 'waiting'),
	(NULL, 'NRouTDxQ31VfP0ZQFyZHbkAL', 'waiting'),
	(NULL, 'Aq6VnCRns908nJd6e28yz4yZ', 'waiting'),
	(NULL, '2msMfNImSZ4trsVjCzJsP3qI', 'waiting'),
	(NULL, 'gIffoVG5h0Tmmwqp23wuedEL', 'waiting'),
	(NULL, 'TyBkIHGVI9fKuUrLgg5O8jyk', 'waiting'),
	(NULL, '7PPxiqFTejPArOuSstzxXTiv', 'waiting'),
	(NULL, 'R5tYDFe9MdoLABrFpamH5IcB', 'waiting'),
	(NULL, 'moaNCOvmTClnaI1dV0YMGzfe', 'waiting'),
	(NULL, 'VlApVirlBFqWHM6ceq5DBN9D', 'waiting'),
	(NULL, '20trf6YqlsOVgljlbM9ejNgT', 'waiting'),
	(NULL, 'cZSJFyxu1d0DjgJB7aDdG4ws', 'waiting'),
	(NULL, 'luuazLW4z61UAymyGPujhGdQ', 'waiting'),
	(NULL, 'lNdIMqFgG4awl2obyVKjjm21', 'waiting'),
	(NULL, 'oVeXUVVzNG7TUm2OKfMLCVkk', 'waiting'),
	(NULL, 'ymBC1vCQ1EBX6Jmw6oqruyGS', 'waiting'),
	(NULL, 'ZkSEK5QD6T8iOHYlkJdEQf0z', 'waiting'),
	(NULL, 'a0WpPZkZ0j4snJowoXfCpOif', 'waiting'),
	(NULL, 'JYztkpAVLnqPwXJ95OHxZ7Hn', 'waiting'),
	(NULL, 't1ZBZEksMHNVImuAhqF3M1Y7', 'waiting'),
	(NULL, 'KmUQYcplJzd2IeEsldne1lwe', 'waiting'),
	(NULL, '3ezpA2ATtNgqZrUHyqUX0u0d', 'waiting'),
	(NULL, 'OLLgPl20CSwJjQmCdD0Lq8Tg', 'waiting'),
	(NULL, '2VTwnKhiMhKT2kOUKwf45CLh', 'waiting'),
	(NULL, 'esyGi6sCURXxEHWT0HIY7a1X', 'waiting'),
	(NULL, 'hxSte2IPZRxqcOE0lkR5b1Np', 'waiting'),
	(NULL, 'YilaeI8vJlQAAZlReb4mx5v2', 'waiting'),
	(NULL, 'pEWVkZhU7qnY6aRUtu7NYwEm', 'waiting'),
	(NULL, '4BxN3b4uzMJVQve8A8pWXmWt', 'waiting'),
	(NULL, '54gmFWOiwypEAa0JeRr0il6E', 'waiting'),
	(NULL, '39Tk6s5pxEi6dbguw4GgfW53', 'waiting'),
	(NULL, 'URheqWLv3w23IdYS9THRdh06', 'waiting'),
	(NULL, 'pv6juu58eqdd9BUaY4uLMqFy', 'waiting'),
	(NULL, 'N1qlrZsRrLitAJ6QRrVqRN18', 'waiting'),
	(NULL, 'CmhI59FrOTfzipiaIDVkByVe', 'waiting'),
	(NULL, 'mBDtoLL5G18zNjor4YulgYDb', 'waiting'),
	(NULL, 'nll7AcR2Lj8gvzF7dVziuGW1', 'waiting'),
	(NULL, 'R6RynqZhZ2t77WMuTyRkq0Ex', 'waiting'),
	(NULL, 'igQZxqmBzVAi6xJDRKYUvWmI', 'waiting'),
	(NULL, 'TR9PRaTkdMV4QChXzXxhJNFF', 'waiting'),
	(NULL, 'K6axt8AL8FAfsrXMV8vCUb9n', 'waiting'),
	(NULL, 'W4YkkvbdnvYdRt2boX6qmbTQ', 'waiting'),
	(NULL, 'FvyayQktZZVHVdF1HsgPg8nD', 'waiting'),
	(NULL, 'baq0Na771kd1lCDIRyq757iF', 'waiting'),
	(NULL, 'JnDqCx6x6E4GfrCp4XztmqEa', 'waiting'),
	(NULL, '9t68QgLfjaCuIF5oqyuNYtM1', 'waiting'),
	(NULL, 'C3bgb992zCVp9nzzoV0SqA3o', 'waiting'),
	(NULL, 'jmN6eiTSk3a2GB35xDDi3tGF', 'waiting'),
	(NULL, 'fqh0JzTy8W4j51M7PqtZF7mC', 'waiting'),
	(NULL, 'ohNRAEqIDW0IpH1yyQVEfT2j', 'waiting'),
	(NULL, 'BilHNOAZIjeRtH1tZj533fYW', 'waiting'),
	(NULL, '8ofgpzX9pURQYBzCeVUwRL2h', 'waiting'),
	(NULL, 'MIBadqUR9t6nYE6y1d7jQ6S0', 'waiting'),
	(NULL, 'V8SROhFmb2V4kYGlHLk3OjMV', 'waiting'),
	(NULL, 'AjOf1SRx54UKhLqCRXgTyT51', 'waiting'),
	(NULL, 'GPvAmOgzuPpKm01cPrMSTE2j', 'waiting'),
	(NULL, 'UU9nG8ahEpSnMS7GgTSTkCRP', 'waiting'),
	(NULL, 'dcork2Qm8ZZ9qb1mSFcosPyf', 'waiting'),
	(NULL, 'jPdecOQ5MbkOBsYkWVzxSAsL', 'waiting'),
	(NULL, 'bLZ4PzsT27WQNHgenYS73tNC', 'waiting'),
	(NULL, 'ulw7zAWEd1FwSa04HJMniu6b', 'waiting'),
	(NULL, 'CZyqcyhhk143kLLxqJ8n2Yjd', 'waiting'),
	(NULL, 'USwnMOQ0sVkmMuuzNvXExgc6', 'waiting'),
	(NULL, 'IXX8UgOCX8RlWlx4Zgdq2mKb', 'waiting'),
	(NULL, 'TmWPk4mR1Hb7gKM3VrqNcTeA', 'waiting'),
	(NULL, 'yGd89lgdMvZVuD9hvRGeC72C', 'waiting'),
	(NULL, 'wStpbEZik7kKew6XeWlAOWiv', 'waiting'),
	(NULL, '57gjHGq5wlKPgu1c4LZp7AYO', 'waiting'),
	(NULL, 'EmPlAQje9bdVvR8WAQeh3JcM', 'waiting'),
	(NULL, 'iRv4cRXTb02Xf4deAicgnyQj', 'waiting'),
	(NULL, '3H0JiiuFbm3jvaTOy9HlVOMl', 'waiting'),
	(NULL, 'bhKTeKU3RqpD3Wnks0opuRcH', 'waiting'),
	(NULL, 'dDZXVSLkJ2Kla8B6SCpXaPrh', 'waiting'),
	(NULL, 'jgK85vP9jfSHt5DaPOvbSEVX', 'waiting'),
	(NULL, 'tFhiamgr2uTFYU8me6zi4VZA', 'waiting'),
	(NULL, '5kVucB5XJRwvKHL6h31mxeqv', 'waiting'),
	(NULL, 'Cvc4Mt2jFwyiCfwOuKJPInme', 'waiting'),
	(NULL, 'fqQoC4TBmC7QErFD1VOJFEsN', 'waiting'),
	(NULL, 'ym57AzYbNqmmdsR3X59xQAyZ', 'waiting'),
	(NULL, 'Zm6ImMmEG7wLO6xItwv1YkSf', 'waiting'),
	(NULL, 'e2l0ubK48IOQQRFtAabgvQII', 'waiting'),
	(NULL, 'W2KM7qzxEITFRR2aBPInI981', 'waiting'),
	(NULL, 'hb2nkUxoGWlJTqCDSIz0Q3iJ', 'waiting'),
	(NULL, 'ZeJ9kUzYT9KbBO2FxcRRmRIU', 'waiting')

END
GO
-- Выполняется этап рефакторинга для обновления развернутых журналов транзакций на целевом сервере

IF OBJECT_ID(N'dbo.__RefactorLog') IS NULL
BEGIN
    CREATE TABLE [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
    EXEC sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
END
GO
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'ff8a7d17-93a2-49a0-9bd6-cfd9f7cf045f')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('ff8a7d17-93a2-49a0-9bd6-cfd9f7cf045f')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '456bdb5a-edfa-4c2e-956e-ede17893f4ee')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('456bdb5a-edfa-4c2e-956e-ede17893f4ee')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '10b92c22-0ac9-4500-962f-8b535d3c3adb')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('10b92c22-0ac9-4500-962f-8b535d3c3adb')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '714e5659-e39a-4e40-a141-6a7286bec7e6')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('714e5659-e39a-4e40-a141-6a7286bec7e6')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'ec1e155e-4e6e-423b-b6f3-4bdfcb0e4336')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('ec1e155e-4e6e-423b-b6f3-4bdfcb0e4336')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '0d588fb6-ab39-4ec9-8c5a-0fafb9218b34')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('0d588fb6-ab39-4ec9-8c5a-0fafb9218b34')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'f11bf446-3134-4214-b5eb-8fdad58e8a0d')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('f11bf446-3134-4214-b5eb-8fdad58e8a0d')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '565d49f5-a2b0-4305-837d-43d8b905f502')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('565d49f5-a2b0-4305-837d-43d8b905f502')

GO

GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
EXEC pr_CreateFkTreeDeleteTrigger 'dbo', 0
EXEC pr_PopulateStaticData

--IF('$(populateTest)' = '1')
--	EXEC pr_PopulateTestData 0
GO

GO
PRINT N'Существующие данные проверяются относительно вновь созданных ограничений';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[AlertMessage] WITH CHECK CHECK CONSTRAINT [FK_AlertMessage_User];

ALTER TABLE [dbo].[Ban] WITH CHECK CHECK CONSTRAINT [FK_Ban_User];

ALTER TABLE [dbo].[Ban] WITH CHECK CHECK CONSTRAINT [FK_Ban_Roles];

ALTER TABLE [dbo].[Ban] WITH CHECK CHECK CONSTRAINT [FK_Ban_User1];

ALTER TABLE [dbo].[Ban] WITH CHECK CHECK CONSTRAINT [FK_Ban_User2];

ALTER TABLE [dbo].[BlackList] WITH CHECK CHECK CONSTRAINT [FK_BlackList_User];

ALTER TABLE [dbo].[BlackList] WITH CHECK CHECK CONSTRAINT [FK_BlackList_User1];

ALTER TABLE [dbo].[Certificates] WITH CHECK CHECK CONSTRAINT [FK_Certificates_User];

ALTER TABLE [dbo].[Cheaters] WITH CHECK CHECK CONSTRAINT [FK_Cheaters_User];

ALTER TABLE [dbo].[GeolocationPlatformPercentage] WITH CHECK CHECK CONSTRAINT [FK_GeolocationPlatformPercentage_Geolocation];

ALTER TABLE [dbo].[GeolocationPlatformPercentage] WITH CHECK CHECK CONSTRAINT [FK_GeolocationPlatformPercentagePerformerPlatform];

ALTER TABLE [dbo].[Image] WITH CHECK CHECK CONSTRAINT [FK_Image_User];

ALTER TABLE [dbo].[Messages] WITH CHECK CHECK CONSTRAINT [FK_Messages_User];

ALTER TABLE [dbo].[Messages] WITH CHECK CHECK CONSTRAINT [FK_Messages_User1];

ALTER TABLE [dbo].[MoneyTransfers] WITH CHECK CHECK CONSTRAINT [FK_MoneyTransfers_User];

ALTER TABLE [dbo].[MoneyTransfers] WITH CHECK CHECK CONSTRAINT [FK_MoneyTransfers_Merchants];

ALTER TABLE [dbo].[News] WITH CHECK CHECK CONSTRAINT [FK_News_User];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_Order_User];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_Order_Regions];

ALTER TABLE [dbo].[OrderContent] WITH CHECK CHECK CONSTRAINT [FK_OrderContent_Order];

ALTER TABLE [dbo].[OrderPerformed] WITH CHECK CHECK CONSTRAINT [FK_OrderPerformed_OrderImages];

ALTER TABLE [dbo].[OrderPerformed] WITH CHECK CHECK CONSTRAINT [FK_OrderPerformed_User];

ALTER TABLE [dbo].[OrderPerformed] WITH CHECK CHECK CONSTRAINT [FK_OrderPerformed_PerformerPlatform];

ALTER TABLE [dbo].[OrderThemes] WITH CHECK CHECK CONSTRAINT [FK_OrderThemes_Order];

ALTER TABLE [dbo].[OrderThemes] WITH CHECK CHECK CONSTRAINT [FK_OrderThemes_Theme];

ALTER TABLE [dbo].[PerformerPlatform] WITH CHECK CHECK CONSTRAINT [FK_PerformerPlatform_Themes];

ALTER TABLE [dbo].[PerformerPlatform] WITH CHECK CHECK CONSTRAINT [FK_PerformerPlatform_User];

ALTER TABLE [dbo].[PerformerStatistics] WITH CHECK CHECK CONSTRAINT [FK_PerformerStatistics_GeolocationPlatformPercentage];

ALTER TABLE [dbo].[Tickets] WITH CHECK CHECK CONSTRAINT [FK_Ticket_Image];

ALTER TABLE [dbo].[Tickets] WITH CHECK CHECK CONSTRAINT [FK_Ticket_Ticket];

ALTER TABLE [dbo].[Tickets] WITH CHECK CHECK CONSTRAINT [FK_Ticket_TicketThemes];

ALTER TABLE [dbo].[Tickets] WITH CHECK CHECK CONSTRAINT [FK_Ticket_User];

ALTER TABLE [dbo].[Tickets] WITH CHECK CHECK CONSTRAINT [FK_Ticket_User1];

ALTER TABLE [dbo].[Tickets] WITH CHECK CHECK CONSTRAINT [FK_Ticket_User2];

ALTER TABLE [dbo].[Transactions] WITH CHECK CHECK CONSTRAINT [FK_Transactions_User];

ALTER TABLE [dbo].[Transactions] WITH CHECK CHECK CONSTRAINT [FK_Transactions_User1];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_Roles];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_Timezone];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_User];

ALTER TABLE [dbo].[UserMerchants] WITH CHECK CHECK CONSTRAINT [FK_UserMerchants_Merchants];

ALTER TABLE [dbo].[UserMerchants] WITH CHECK CHECK CONSTRAINT [FK_UserMerchants_User];

ALTER TABLE [dbo].[Video] WITH CHECK CHECK CONSTRAINT [FK_Video_User];

ALTER TABLE [dbo].[Wallet] WITH CHECK CHECK CONSTRAINT [FK_Wallet_User];

ALTER TABLE [dbo].[WhiteList] WITH CHECK CHECK CONSTRAINT [FK_WhiteList_User];

ALTER TABLE [dbo].[WhiteList] WITH CHECK CHECK CONSTRAINT [FK_WhiteList_User1];


GO
DECLARE @VarDecimalSupported AS BIT;

SELECT @VarDecimalSupported = 0;

IF ((ServerProperty(N'EngineEdition') = 3)
    AND (((@@microsoftversion / power(2, 24) = 9)
          AND (@@microsoftversion & 0xffff >= 3024))
         OR ((@@microsoftversion / power(2, 24) = 10)
             AND (@@microsoftversion & 0xffff >= 1600))))
    SELECT @VarDecimalSupported = 1;

IF (@VarDecimalSupported > 0)
    BEGIN
        EXECUTE sp_db_vardecimal_storage_format N'$(DatabaseName)', 'ON';
    END


GO
PRINT N'Обновление завершено.';


GO
