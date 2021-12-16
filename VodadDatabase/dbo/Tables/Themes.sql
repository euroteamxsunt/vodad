CREATE TABLE [dbo].[Themes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL, 
    CONSTRAINT [PK_Themes] PRIMARY KEY ([Id])
);
GO