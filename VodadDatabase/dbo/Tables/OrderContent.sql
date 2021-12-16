CREATE TABLE [dbo].[OrderContent] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [IdOrder] bigint  NOT NULL,
    [IdContent] bigint  NOT NULL,
    [Status] nvarchar(50)  NULL, 
    [ContentType] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_OrderContent] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_FK_OrderContent_Order]
ON [dbo].[OrderContent]
    ([IdOrder]);
GO

ALTER TABLE [dbo].[OrderContent]
ADD CONSTRAINT [FK_OrderContent_Order]
    FOREIGN KEY ([IdOrder])
    REFERENCES [dbo].[Order]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
