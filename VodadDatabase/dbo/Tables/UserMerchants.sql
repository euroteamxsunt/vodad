CREATE TABLE [dbo].[UserMerchants] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] BIGINT  NULL,
	[MerchantId] BIGINT NULL, 
    [Account] NVARCHAR(50) NULL, 
    [PreviousAccount] NVARCHAR(50) NULL, 
    [NextAccount] NVARCHAR(50) NULL, 
    [Status] NVARCHAR(50) NULL, 
    [LastChangeDateTime] DATETIME NULL, 
    CONSTRAINT [PK_UserMerchants] PRIMARY KEY ([Id])
    
);
GO

CREATE INDEX [IX_FK_UserMerchants_Merchants]
ON [dbo].[UserMerchants]
    ([MerchantId]);
GO

ALTER TABLE [dbo].[UserMerchants]
ADD CONSTRAINT [FK_UserMerchants_Merchants]
    FOREIGN KEY ([MerchantId])
    REFERENCES [dbo].[Merchants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_UserMerchants_User]
ON [dbo].[UserMerchants]
    ([UserId]);
GO

ALTER TABLE [dbo].[UserMerchants]
ADD CONSTRAINT [FK_UserMerchants_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO