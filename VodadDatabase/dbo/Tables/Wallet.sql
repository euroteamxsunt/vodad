CREATE TABLE [dbo].[Wallet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NULL,
    [Account] decimal(25,13)  NULL,
    [Transfer] decimal(25,13)  NULL,
    [ReferralsIncome] decimal(25,13)  NULL, 
    CONSTRAINT [PK_Wallet] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[Wallet]
ADD CONSTRAINT [FK_Wallet_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Wallet_User]
ON [dbo].[Wallet]
    ([UserId]);