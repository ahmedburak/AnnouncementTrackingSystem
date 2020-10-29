CREATE TABLE [dbo].[Announcements] (
    [PkId]   INT             IDENTITY (1, 1) NOT NULL,
    [TypeId] TINYINT         NOT NULL,
    [Text]   NVARCHAR (4000) NOT NULL,
    [Url]    NVARCHAR (500)  NOT NULL,
    [Date]   DATETIME        NOT NULL,
    CONSTRAINT [PK_Announcements] PRIMARY KEY CLUSTERED ([PkId] ASC),
    CONSTRAINT [FK_Announcements_AnnouncementTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[AnnouncementTypes] ([PkId])
);

