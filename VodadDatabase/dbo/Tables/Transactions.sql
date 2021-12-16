CREATE TABLE [dbo].[Transactions] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [FromUserId] bigint  NOT NULL,
    [Amount] decimal(24,13)  NOT NULL,
    [ToUserId] bigint  NOT NULL,
    [DateTime] datetime  NOT NULL, 
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_Transactions_User]
    FOREIGN KEY ([FromUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Transactions]
ADD CONSTRAINT [FK_Transactions_User1]
    FOREIGN KEY ([ToUserId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Transactions_User]
ON [dbo].[Transactions]
    ([FromUserId]);
GO

CREATE INDEX [IX_FK_Transactions_User1]
ON [dbo].[Transactions]
    ([ToUserId]);