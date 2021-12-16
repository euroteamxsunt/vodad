CREATE TABLE [dbo].[User] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Login] nvarchar(50)  NULL,
    [Email] nvarchar(100)  NULL,
    [Password] nvarchar(128)  NULL,
    [RoleId] bigint  NULL,
    [RegistrationDate] datetime  NULL,
    [TimeZoneId] bigint  NULL,
    [ReferrerId] bigint  NULL,
    [Status] nvarchar(50)  NULL,
    [PasswordSalt] nvarchar(128)  NOT NULL,
    [Comments] nvarchar(4000)  NULL,
    [LastModifiedDate] datetime  NULL,
    [LastLoginDate] datetime  NOT NULL,
    [LastLoginIp] nvarchar(40)  NULL,
    [IsActivated] bit  NOT NULL,
    [IsLockedOut] bit  NOT NULL,
    [LastLockedOutDate] datetime  NOT NULL,
    [LastLockedOutReason] nvarchar(256)  NULL,
    [NewPasswordKey] nvarchar(128)  NULL,
    [NewEmail] nvarchar(100)  NULL,
    [NewEmailKey] nvarchar(128)  NULL,
    [NewEmailRequested] datetime  NULL,
    [Rating] bigint  NULL,
    [Karma] bigint  NULL,
    [LastOnlineDateTime] datetime  NULL, 
    [LastEmailSendDateTime] DATETIME NULL, 
    [AccountAttachedIP] NVARCHAR(40) NULL, 
    [RegisteredRoleId] BIGINT NULL, 
    [VerificationKey] NVARCHAR(128) NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_Timezone]
    FOREIGN KEY ([TimeZoneId])
    REFERENCES [dbo].[Timezone]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_User]
    FOREIGN KEY ([ReferrerId])
    REFERENCES [dbo].[User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_User_Roles]
ON [dbo].[User]
    ([RoleId]);
GO

CREATE INDEX [IX_FK_User_Timezone]
ON [dbo].[User]
    ([TimeZoneId]);
GO

CREATE INDEX [IX_FK_User_User]
ON [dbo].[User]
    ([ReferrerId]);