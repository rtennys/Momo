using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Momo.Common.DataAccess
{
    public class InMemoryRepository : IRepository
    {
        public T Load<T>(object id) where T : IEntity
        {
            return Get<T>(id);
        }

        public T Get<T>(object id) where T : IEntity
        {
            return Get<T>(x => Equals(x.Id, id));
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return Find<T>().SingleOrDefault(predicate);
        }

        public IQueryable<T> Find<T>() where T : IEntity
        {
            return DataStore.ListFor<T>().AsQueryable();
        }

        public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return Find<T>().Where(predicate);
        }

        public T Add<T>(T entity) where T : IEntity
        {
            IdGenerator.SetId(entity);
            DataStore.ListFor<T>().Add(entity);
            return entity;
        }

        public T Remove<T>(T entity) where T : IEntity
        {
            DataStore.ListFor<T>().Remove(entity);
            return entity;
        }

        public void Remove<T>(object id) where T : IEntity
        {
            Remove(Get<T>(id));
        }

        public static class DataStore
        {
            private static readonly IDictionary<Type, object> _lists = new Dictionary<Type, object>();
            private static readonly object _lockObject = new object();

            public static IList<T> ListFor<T>()
            {
                var type = typeof(T);

                if (!_lists.ContainsKey(type))
                {
                    lock (_lockObject)
                    {
                        if (!_lists.ContainsKey(type))
                        {
                            var listType = typeof(List<>).MakeGenericType(type);
                            _lists[type] = Activator.CreateInstance(listType);
                        }
                    }
                }

                return (IList<T>)_lists[type];
            }

            public static void Clear()
            {
                _lists.Clear();
            }
        }
    }
}