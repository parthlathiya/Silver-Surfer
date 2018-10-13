CREATE TABLE [dbo].[admin] (
    [username] VARCHAR (200) NOT NULL,
    [password] VARCHAR (200) NOT NULL,
    [name]     VARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([username] ASC)
);

