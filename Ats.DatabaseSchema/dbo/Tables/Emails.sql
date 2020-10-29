CREATE TABLE [dbo].[Emails] (
    [PkId]         INT            IDENTITY (1, 1) NOT NULL,
    [EmailAddress] NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_Emails] PRIMARY KEY CLUSTERED ([PkId] ASC)
);

