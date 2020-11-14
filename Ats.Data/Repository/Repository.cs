using Ats.Model.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ats.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AtsContext context;

        public Repository(AtsContext database)
        {
            context = database;
        }

        public List<T> GetAll()
        {
            return context.Set<T>().AsNoTracking().ToList();
        }

        public List<T> GetAllForUpdate()
        {
            return context.Set<T>().ToList();
        }

        public List<T> Search(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate).AsNoTracking().ToList();
        }

        public List<T> SearchForUpdate(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate).ToList();
        }

        public T Insert(T entity)
        {
            context.Set<T>().Add(entity);

            return entity;
        }

        public List<T> Insert(List<T> entities)
        {
            context.Set<T>().AddRange(entities);

            return entities;
        }

        public T Single(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().AsNoTracking().FirstOrDefault(predicate);
        }

        public T SingleForUpdate(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().FirstOrDefault(predicate);
        }

        public T SingleForUpdate(int id)
        {
            return context.Set<T>().Find(id);
        }

        public T Update(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public void Delete(T entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }

        public void DeleteAll(List<T> entities)
        {
            foreach (var item in entities)
            {
                context.Entry(item).State = EntityState.Deleted;
            }
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Any(predicate);
        }
    }
}