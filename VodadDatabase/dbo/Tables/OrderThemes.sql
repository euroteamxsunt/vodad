CREATE TABLE [dbo].[OrderThemes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [OrderId] bigint  NULL,
    [ThemeId] bigint  NULL, 
    CONSTRAINT [PK_OrderThemes] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[OrderThemes]
ADD CONSTRAINT [FK_OrderThemes_Order]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[Order]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[OrderThemes]
ADD CONSTRAINT [FK_OrderThemes_Theme]
    FOREIGN KEY ([ThemeId])
    REFERENCES [dbo].[Themes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_OrderThemes_Order]
ON [dbo].[OrderThemes]
    ([OrderId]);
GO

CREATE INDEX [IX_FK_OrderThemes_Theme]
ON [dbo].[OrderThemes]
    ([ThemeId]);