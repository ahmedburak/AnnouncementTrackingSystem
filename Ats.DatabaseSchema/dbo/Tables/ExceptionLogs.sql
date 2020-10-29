CREATE TABLE [dbo].[ExceptionLogs] (
    [PkId]              INT             NOT NULL,
    [Message]           NVARCHAR (4000) NOT NULL,
    [SpecialMessage]    NVARCHAR (4000) NULL,
    [InnerException]    NVARCHAR (4000) NULL,
    [StackTrace]        NVARCHAR (4000) NOT NULL,
    [ExceptionDateTime] DATETIME        NOT NULL
);

