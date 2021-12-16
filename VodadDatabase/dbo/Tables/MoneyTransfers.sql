CREATE TABLE [dbo].[MoneyTransfers] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [DateTime] datetime  NULL,
    [Action] nvarchar(50)  NULL,
    [Amount] decimal(25,13)  NULL, 
    [MerchantId] BIGINT NULL, 
    [AccountMerchantSystem] NVARCHAR(50) NULL, 
    [TransactionId] NVARCHAR(50) NULL, 
    [Status] NVARCHAR(50) NULL, 
    [StatusDescription] NVARCHAR(4000) NULL, 
    CONSTRAINT [PK_MoneyTransfers] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_FK_MoneyTransfers_User]
ON [dbo].[MoneyTransfers]
    ([UserId]);
GO

CREATE INDEX [IX_FK_MoneyTransfers_Merchants]
ON [dbo].[MoneyTransfers]
    ([MerchantId]);
GO

ALTER TABLE [dbo].[MoneyTransfers]
ADD CONSTRAINT [FK_MoneyTransfers_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[MoneyTransfers]
ADD CONSTRAINT [FK_MoneyTransfers_Merchants]
    FOREIGN KEY ([MerchantId])
    REFERENCES [dbo].[Merchants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO