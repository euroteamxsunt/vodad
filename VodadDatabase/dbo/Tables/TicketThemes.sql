CREATE TABLE [dbo].[TicketThemes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ThemeName] nvarchar(50)  NULL, 
    CONSTRAINT [PK_TicketThemes] PRIMARY KEY ([Id])
);
GO