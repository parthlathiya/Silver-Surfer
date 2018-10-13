CREATE TABLE [dbo].[subscriptionpack] (
    [sid]    INT NOT NULL,
    [packid] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([sid] ASC, [packid] ASC),
    FOREIGN KEY ([packid]) REFERENCES [dbo].[pack] ([packid]),
    FOREIGN KEY ([sid]) REFERENCES [dbo].[subscriptionmaster] ([sid])
);

