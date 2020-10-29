CREATE TABLE [dbo].[AnnouncementDefinitions] (
    [PkId]             INT            IDENTITY (1, 1) NOT NULL,
    [Url]              NVARCHAR (500) NOT NULL,
    [TypeId]           TINYINT        NOT NULL,
    [RowCssSelector]   NVARCHAR (500) NOT NULL,
    [ClickCssSelector] NVARCHAR (500) NOT NULL,
    [Description]      NVARCHAR (500) NOT NULL,
    CONSTRAINT [PK_Pages] PRIMARY KEY CLUSTERED ([PkId] ASC),
    CONSTRAINT [FK_AnnouncementDefinitions_AnnouncementTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[AnnouncementTypes] ([PkId])
);

