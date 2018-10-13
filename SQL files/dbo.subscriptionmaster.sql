CREATE TABLE [dbo].[subscriptionmaster] (
    [sid]        INT           IDENTITY (1, 1) NOT NULL,
    [email]      VARCHAR (200) NOT NULL,
    [cost]       FLOAT (53)    NOT NULL,
    [expirydate] DATETIME      NOT NULL,
    [transid]    VARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([sid] ASC),
    FOREIGN KEY ([email]) REFERENCES [dbo].[Customer] ([email])
);

