CREATE TABLE [dbo].[subscriptionchannel] (
    [sid]       INT NOT NULL,
    [channelid] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([sid] ASC, [channelid] ASC),
    FOREIGN KEY ([channelid]) REFERENCES [dbo].[channel] ([channelid]),
    FOREIGN KEY ([sid]) REFERENCES [dbo].[subscriptionmaster] ([sid])
);

