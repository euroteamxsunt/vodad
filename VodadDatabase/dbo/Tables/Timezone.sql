CREATE TABLE [dbo].[Timezone] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NULL, 
    CONSTRAINT [PK_Timezone] PRIMARY KEY ([Id])
);
GO