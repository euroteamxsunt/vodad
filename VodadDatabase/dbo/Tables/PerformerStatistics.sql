CREATE TABLE [dbo].[PerformerStatistics] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [PerformerPlatformId] bigint  NULL,
    [AverageViewerCountPerHour] bigint  NULL,
    [MaxViewersCount] bigint  NULL,
    [TotalUniqueViews] bigint  NULL,
    [TotalFollowers] bigint  NULL,
    [TotalViews] decimal(18,0)  NULL,
    [Likes] bigint  NULL,
    [CompletedOrders] bigint  NULL,
    [TotalOrders] bigint  NULL,
    [AverageComplitionSpeed] decimal(18,0)  NULL,
    [DateModified] datetime  NOT NULL,
    [Status] nvarchar(max)  NULL, 
    [UniqueViewersForMonth] BIGINT NULL, 
    CONSTRAINT [PK_PerformerStatistics] PRIMARY KEY ([Id])
);
GO

ALTER TABLE [dbo].[PerformerStatistics]
ADD CONSTRAINT [FK_PerformerStatistics_GeolocationPlatformPercentage]
    FOREIGN KEY ([PerformerPlatformId])
    REFERENCES [dbo].[PerformerPlatform]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_PerformerStatistics_GeolocationPlatformPercentage]
ON [dbo].[PerformerStatistics]
    ([PerformerPlatformId]);