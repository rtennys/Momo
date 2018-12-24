using System;
using System.Linq;
using System.Linq.Expressions;

namespace Momo.Domain
{
    public interface IRepository
    {
        T Load<T>(int id) where T : Entity;
        T Get<T>(int id) where T : Entity;
        T Get<T>(Expression<Func<T, bool>> predicate) where T : Entity;
        IQueryable<T> Find<T>() where T : Entity;
        IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : Entity;
        T Add<T>(T entity) where T : Entity;
        T Remove<T>(T entity) where T : Entity;
        void Remove<T>(int id) where T : Entity;
    }
}
