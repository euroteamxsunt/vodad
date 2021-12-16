CREATE TABLE [dbo].[GeolocationPlatformPercentage] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [GeolocationId] bigint  NULL,
    [Percentage] bigint  NULL,
    [PerformerPlatformId] bigint  NOT NULL, 
    CONSTRAINT [PK_GeolocationPlatformPercentage] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[GeolocationPlatformPercentage]
ADD CONSTRAINT [FK_GeolocationPlatformPercentage_Geolocation]
    FOREIGN KEY ([GeolocationId])
    REFERENCES [dbo].[Geolocation]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

ALTER TABLE [dbo].[GeolocationPlatformPercentage]
ADD CONSTRAINT [FK_GeolocationPlatformPercentagePerformerPlatform]
    FOREIGN KEY ([PerformerPlatformId])
    REFERENCES [dbo].[PerformerPlatform]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_GeolocationPlatformPercentage_Geolocation]
ON [dbo].[GeolocationPlatformPercentage]
    ([GeolocationId]);
GO

CREATE INDEX [IX_FK_GeolocationPlatformPercentagePerformerPlatform]
ON [dbo].[GeolocationPlatformPercentage]
    ([PerformerPlatformId]);