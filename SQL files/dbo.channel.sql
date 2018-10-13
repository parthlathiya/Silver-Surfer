CREATE TABLE [dbo].[channel] (
    [channelid] INT           IDENTITY (1, 1) NOT NULL,
    [name]      VARCHAR (200) NOT NULL,
    [cost]      FLOAT (53)    NULL,
    PRIMARY KEY CLUSTERED ([channelid] ASC)
);

