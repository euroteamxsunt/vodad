CREATE TABLE [dbo].[Cheaters] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] BIGINT  NULL, 
    CONSTRAINT [PK_Cheaters] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[Cheaters]
ADD CONSTRAINT [FK_Cheaters_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Cheaters_User]
ON [dbo].[Cheaters]
    ([UserId]);