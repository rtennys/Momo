using System;
using System.Linq;
using System.Linq.Expressions;

namespace Momo.Common.DataAccess
{
    public interface IRepository
    {
        /// <summary>
        /// Catering to nhibernate here...
        /// Loads a proxy object with nothing but the primary key set.  
        /// Other properties will be pulled from the DB the first time they are accessed.
        /// Only use when you know you will NOT be wanting the other properties though.
        /// Useful for setting foreign keys or getting an entity merely to delete it.
        /// </summary>
        T Load<T>(object id) where T : IEntity;

        T Get<T>(object id) where T : IEntity;
        T Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity;

        IQueryable<T> Find<T>() where T : IEntity;
        IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IEntity;

        T Add<T>(T entity) where T : IEntity;
        T Remove<T>(T entity) where T : IEntity;
        void Remove<T>(object id) where T : IEntity;
    }
}