CREATE TABLE [dbo].[Geolocation] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CountryName] nvarchar(50)  NULL,
    [ISO2] nvarchar(max)  NULL,
    [LongCountryName] nvarchar(max)  NULL,
    [ISO3] nvarchar(max)  NULL,
    [NumCode] nvarchar(max)  NULL,
    [UNMemberState] nvarchar(max)  NULL,
    [CallingCode] nvarchar(max)  NULL,
    [CCTLD] nvarchar(max)  NULL, 
    CONSTRAINT [PK_Geolocation] PRIMARY KEY ([Id])
);
GO