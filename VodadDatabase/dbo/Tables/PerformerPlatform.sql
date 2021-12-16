CREATE TABLE [dbo].[PerformerPlatform] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [PerformerId] bigint  NULL,
    [Login] nvarchar(50)  NULL,
    [Password] nvarchar(50)  NULL,
    [Status] nvarchar(50)  NULL,
    [ThemeId] bigint  NULL,
    [Verified] nvarchar(50)  NULL,
    [LastOnlineDateTime] datetime  NULL,
    [Link] NVARCHAR(200) NULL, 
    [Date] DATETIME NULL, 
    [ChannelName] NVARCHAR(50) NULL, 
    [VideoPrice] DECIMAL(25, 13) NULL, 
    [LogoPrice] DECIMAL(25, 13) NULL, 
    CONSTRAINT [PK_PerformerPlatform] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[PerformerPlatform]
ADD CONSTRAINT [FK_PerformerPlatform_Themes]
    FOREIGN KEY ([ThemeId])
    REFERENCES [dbo].[Themes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[PerformerPlatform]
ADD CONSTRAINT [FK_PerformerPlatform_User]
    FOREIGN KEY ([PerformerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_PerformerPlatform_Themes]
ON [dbo].[PerformerPlatform]
    ([ThemeId]);
GO

CREATE INDEX [IX_FK_PerformerPlatform_User]
ON [dbo].[PerformerPlatform]
    ([PerformerId]);