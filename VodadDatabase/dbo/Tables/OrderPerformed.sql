CREATE TABLE [dbo].[OrderPerformed] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Status] nvarchar(50)  NULL,
    [MoneyPaid] decimal(25,13)  NULL,
    [OrderContentId] bigint  NULL,
    [AuthorId] bigint  NOT NULL,
    [IsLiked] bit  NULL, 
    [PerformerPlatformId] BIGINT NULL, 
    [VideoLink] NVARCHAR(400) NULL, 
    [StartDate] DATETIME NULL, 
    [LastStatusChangeDateTime] DATETIME NULL, 
    CONSTRAINT [PK_OrderPerformed] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[OrderPerformed]
ADD CONSTRAINT [FK_OrderPerformed_OrderImages]
    FOREIGN KEY ([OrderContentId])
    REFERENCES [dbo].[OrderContent]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[OrderPerformed]
ADD CONSTRAINT [FK_OrderPerformed_User]
    FOREIGN KEY ([AuthorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[OrderPerformed]
ADD CONSTRAINT [FK_OrderPerformed_PerformerPlatform]
    FOREIGN KEY ([PerformerPlatformId])
    REFERENCES [dbo].[PerformerPlatform]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_OrderPerformed_OrderContent]
ON [dbo].[OrderPerformed]
    ([OrderContentId]);
GO

CREATE INDEX [IX_FK_OrderPerformed_User]
ON [dbo].[OrderPerformed]
    ([AuthorId]);
GO

CREATE INDEX [IX_FK_OrderPerformed_PerformerPlatform]
ON [dbo].[OrderPerformed]
    ([PerformerPlatformId]);
GO