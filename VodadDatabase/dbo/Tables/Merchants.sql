CREATE TABLE [dbo].[Merchants] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [MerchantName] NVARCHAR(50)  NULL, 
    CONSTRAINT [PK_Merchants] PRIMARY KEY ([Id])
    , 
);
GO