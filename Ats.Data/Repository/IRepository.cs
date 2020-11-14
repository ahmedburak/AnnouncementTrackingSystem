using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ats.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        // List
        List<T> GetAll();
        List<T> GetAllForUpdate();
        List<T> Search(Expression<Func<T, bool>> predicate);
        List<T> SearchForUpdate(Expression<Func<T, bool>> predicate);


        // Get
        T Single(Expression<Func<T, bool>> predicate);
        T SingleForUpdate(Expression<Func<T, bool>> predicate);
        T SingleForUpdate(int id);


        // Add
        T Insert(T entity);


        // Edit
        T Update(T entity);


        // Delete
        void Delete(T entity);
        void DeleteAll(List<T> entities);

        // Search
        bool Any(Expression<Func<T, bool>> predicate);
    }
}