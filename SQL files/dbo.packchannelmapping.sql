CREATE TABLE [dbo].[packchannelmapping] (
    [packid]    INT NOT NULL,
    [channelid] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([packid] ASC, [channelid] ASC),
    FOREIGN KEY ([channelid]) REFERENCES [dbo].[channel] ([channelid]),
    FOREIGN KEY ([packid]) REFERENCES [dbo].[pack] ([packid])
);

