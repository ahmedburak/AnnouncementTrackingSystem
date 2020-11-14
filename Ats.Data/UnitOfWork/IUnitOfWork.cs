using Ats.Data.Repository;
using Ats.Model.Models;

using System;

namespace Ats.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Repository<Announcement> Announcements { get; }
        Repository<AnnouncementDefinition> AnnouncementDefinitions { get; }
        Repository<AnnouncementType> AnnouncementTypes { get; }
        Repository<Email> Emails { get; }
        Repository<ExceptionLog> ExceptionLogs { get; }


        int Commit();
        void Rollback();
    }
}