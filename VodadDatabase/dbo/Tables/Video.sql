CREATE TABLE [dbo].[Video] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NOT NULL,
    [VideoName] nvarchar(255)  NOT NULL,
    [VideoLink] nvarchar(4000)  NOT NULL,
    [VideoSize] bigint  NOT NULL,
	[VideoLength] bigint NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [Extension] NVARCHAR(40) NULL, 
    CONSTRAINT [PK_Video] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_FK_Video_User]
ON [dbo].[Video]
    ([UserId]);
GO

ALTER TABLE [dbo].[Video]
ADD CONSTRAINT [FK_Video_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;