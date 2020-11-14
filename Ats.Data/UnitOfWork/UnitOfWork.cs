using Ats.Data.Repository;
using Ats.Model.Models;

using Microsoft.EntityFrameworkCore.Storage;

using System;

namespace Ats.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        // Privates
        private IDbContextTransaction transaction;
        private readonly AtsContext db;


        // Entities
        public Repository<Announcement> Announcements => new Repository<Announcement>(db);

        public Repository<AnnouncementDefinition> AnnouncementDefinitions => new Repository<AnnouncementDefinition>(db);

        public Repository<AnnouncementType> AnnouncementTypes => new Repository<AnnouncementType>(db);

        public Repository<Email> Emails => new Repository<Email>(db);

        public Repository<ExceptionLog> ExceptionLogs => new Repository<ExceptionLog>(db);


        // Actions
        public int Commit()
        {
            transaction = db.Database.BeginTransaction();
            var result = db.SaveChanges();
            transaction.Commit();
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            if (db != null)
            {
                db.Dispose();
            }
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Rollback()
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }
        }
    }
}