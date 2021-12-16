CREATE TABLE [dbo].[Certificates] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] BIGINT  NULL, 
    [Code] NVARCHAR(50) NULL, 
    [Status] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[Certificates]
ADD CONSTRAINT [FK_Certificates_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Certificates_User]
ON [dbo].[Certificates]
    ([UserId]);