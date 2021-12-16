CREATE TABLE [dbo].[Messages] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [FromUserId] bigint  NULL,
    [ToUserId] bigint  NULL,
    [CreationDate] datetime  NULL,
    [MessageText] nvarchar(4000)  NULL,
    [IsRead] bit  NULL,
    [MessageTitle] nvarchar(50)  NULL,
    [IsDeletedForAuthor] bit  NULL,
    [IsDeletedForReciever] bit  NULL, 
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_Messages_User]
    FOREIGN KEY ([FromUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [FK_Messages_User1]
    FOREIGN KEY ([ToUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Messages_User]
ON [dbo].[Messages]
    ([FromUserId]);
GO

CREATE INDEX [IX_FK_Messages_User1]
ON [dbo].[Messages]
    ([ToUserId]);