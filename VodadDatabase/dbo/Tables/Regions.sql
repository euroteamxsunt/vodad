CREATE TABLE [dbo].[Regions] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RegionName] nvarchar(50)  NULL, 
    CONSTRAINT [PK_Regions] PRIMARY KEY ([Id])
);
GO