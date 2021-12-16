CREATE TABLE [dbo].[Ban] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [RoleId] bigint  NULL,
    [BanDateTime] datetime  NULL, 
    [UnbanDateTime] DATETIME NULL, 
    [BanReason] NVARCHAR(4000) NULL, 
    [UnbanReason] NVARCHAR(4000) NULL, 
    [CanBeUnbanned] BIT NULL, 
    [AdminBanId] BIGINT NULL, 
    [AdminUnbanId] BIGINT NULL, 
    CONSTRAINT [PK_Ban] PRIMARY KEY ([Id]), 
);
GO

ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_User1]
    FOREIGN KEY ([AdminBanId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Ban]
ADD CONSTRAINT [FK_Ban_User2]
    FOREIGN KEY ([AdminUnbanId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Ban_User]
ON [dbo].[Ban]
    ([UserId]);
GO

CREATE INDEX [IX_FK_Ban_Roles]
ON [dbo].[Ban]
    ([RoleId]);
GO

CREATE INDEX [IX_FK_Ban_User1]
ON [dbo].[Ban]
    ([AdminBanId]);
GO

CREATE INDEX [IX_FK_Ban_User2]
ON [dbo].[Ban]
    ([AdminUnbanId]);
GO