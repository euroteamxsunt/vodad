CREATE TABLE [dbo].[WhiteList] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OwnerId] bigint  NULL,
    [UserId] bigint  NULL, 
    CONSTRAINT [PK_WhiteList] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[WhiteList]
ADD CONSTRAINT [FK_WhiteList_User]
    FOREIGN KEY ([OwnerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[WhiteList]
ADD CONSTRAINT [FK_WhiteList_User1]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_WhiteList_User]
ON [dbo].[WhiteList]
    ([OwnerId]);
GO

CREATE INDEX [IX_FK_WhiteList_User1]
ON [dbo].[WhiteList]
    ([UserId]);