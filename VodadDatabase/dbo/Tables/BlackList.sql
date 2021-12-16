CREATE TABLE [dbo].[BlackList] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OwnerId] bigint  NULL,
    [UserId] bigint  NULL, 
    CONSTRAINT [PK_BlackList] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[BlackList]
ADD CONSTRAINT [FK_BlackList_User]
    FOREIGN KEY ([OwnerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[BlackList]
ADD CONSTRAINT [FK_BlackList_User1]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_BlackList_User]
ON [dbo].[BlackList]
    ([OwnerId]);
GO

CREATE INDEX [IX_FK_BlackList_User1]
ON [dbo].[BlackList]
    ([UserId]);