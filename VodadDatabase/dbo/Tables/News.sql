CREATE TABLE [dbo].[News] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreatorId] bigint  NULL,
    [Title] nvarchar(100)  NULL,
    [Text] nvarchar(4000)  NULL,
    [Text1] nvarchar(4000)  NULL,
    [Text2] nvarchar(4000)  NULL,
    [CreationDate] datetime  NULL, 
    CONSTRAINT [PK_News] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[News]
ADD CONSTRAINT [FK_News_User]
    FOREIGN KEY ([CreatorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_News_User]
ON [dbo].[News]
    ([CreatorId]);