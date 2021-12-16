CREATE TABLE [dbo].[Roles] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(50)  NULL, 
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO