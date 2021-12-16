-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------
CREATE TABLE [dbo].[AlertMessage] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreatorId] bigint  NULL,
    [Text] nvarchar(500)  NULL,
    [CreationDate] datetime  NULL, 
    CONSTRAINT [PK_AlertMessage] PRIMARY KEY ([Id]) 
);
GO

ALTER TABLE [dbo].[AlertMessage]
ADD CONSTRAINT [FK_AlertMessage_User]
    FOREIGN KEY ([CreatorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_AlertMessage_User]
ON [dbo].[AlertMessage]
    ([CreatorId]);