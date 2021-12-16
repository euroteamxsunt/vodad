CREATE TABLE [dbo].[Tickets] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(500)  NULL,
    [ThemeId] bigint  NULL,
    [Text1] nvarchar(4000)  NULL,
    [Text2] nvarchar(4000)  NULL,
    [Text3] nvarchar(4000)  NULL,
    [ImageId] bigint  NULL,
    [CreationDate] datetime  NULL,
    [AnswerDate] datetime  NULL,
    [CloseDate] datetime  NULL,
    [ParentTicketId] bigint  NULL,
    [AdminAnswer] nvarchar(4000)  NULL,
    [AdminCloseComment] nvarchar(4000)  NULL,
    [CreatorId] bigint  NULL,
    [AnswerAdminId] bigint  NULL,
    [CloseAdminId] bigint  NULL,
    [Status] nvarchar(50)  NULL, 
    CONSTRAINT [PK_Tickets] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_Image]
    FOREIGN KEY ([ImageId])
    REFERENCES [dbo].[Image]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_Ticket]
    FOREIGN KEY ([ParentTicketId])
    REFERENCES [dbo].[Tickets]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_TicketThemes]
    FOREIGN KEY ([ThemeId])
    REFERENCES [dbo].[TicketThemes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_User]
    FOREIGN KEY ([CreatorId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_User1]
    FOREIGN KEY ([AnswerAdminId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_Ticket_User2]
    FOREIGN KEY ([CloseAdminId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_Ticket_Image]
ON [dbo].[Tickets]
    ([ImageId]);
GO

CREATE INDEX [IX_FK_Ticket_Ticket]
ON [dbo].[Tickets]
    ([ParentTicketId]);
GO

CREATE INDEX [IX_FK_Ticket_TicketThemes]
ON [dbo].[Tickets]
    ([ThemeId]);
GO

CREATE INDEX [IX_FK_Ticket_User]
ON [dbo].[Tickets]
    ([CreatorId]);
GO

CREATE INDEX [IX_FK_Ticket_User1]
ON [dbo].[Tickets]
    ([AnswerAdminId]);
GO

CREATE INDEX [IX_FK_Ticket_User2]
ON [dbo].[Tickets]
    ([CloseAdminId]);