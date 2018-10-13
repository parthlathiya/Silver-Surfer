CREATE TABLE [dbo].[Customer] (
    [email]     VARCHAR (200) NOT NULL,
    [name]      VARCHAR (200) NULL,
    [contactno] VARCHAR (200) NULL,
    [address]   VARCHAR (200) NULL,
    [dob]       VARCHAR (200) NULL,
    [password]  VARCHAR (200) NOT NULL,
    [profileid] VARCHAR (200) NULL,
    [paymentid] VARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([email] ASC)
);

