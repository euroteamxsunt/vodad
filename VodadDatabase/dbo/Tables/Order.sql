CREATE TABLE [dbo].[Order] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [Comment] nvarchar(4000)  NULL,
    [CreationDate] datetime  NULL,
    [ExpireDate] datetime  NULL,
    [Status] nvarchar(50)  NULL,
    [RegionId] bigint  NULL, 
    [Name] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_Order] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_FK_Order_User]
ON [dbo].[Order]
    ([UserId]);
GO

CREATE INDEX [IX_FK_Order_Regions]
ON [dbo].[Order]
    ([RegionId]);
GO

ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_Order_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Order]
ADD CONSTRAINT [FK_Order_Regions]
    FOREIGN KEY ([RegionId])
    REFERENCES [dbo].[Regions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;