CREATE TABLE [dbo].[Image] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NOT NULL,
    [ImageName] nvarchar(255)  NOT NULL,
    [ImageData] varbinary(max)  NOT NULL,
    [ImageSize] bigint  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [ImageWidth] int  NULL,
    [ImageHeight] int  NULL, 
    [Extension] NVARCHAR(40) NULL, 
    CONSTRAINT [PK_Image] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_FK_Image_User]
ON [dbo].[Image]
    ([UserId]);
GO

ALTER TABLE [dbo].[Image]
ADD CONSTRAINT [FK_Image_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;