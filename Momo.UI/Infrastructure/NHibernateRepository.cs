using System;
using System.Linq;
using System.Linq.Expressions;
using Momo.Common.DataAccess;
using NHibernate.Linq;

namespace Momo.UI.Infrastructure
{
    public class NHibernateRepository : IRepository
    {
        public NHibernateRepository(INHibernateUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly INHibernateUnitOfWork _unitOfWork;

        public T Load<T>(object id) where T : IEntity
        {
            return _unitOfWork.CurrentSession.Load<T>(id);
        }

        public T Get<T>(object id) where T : IEntity
        {
            return _unitOfWork.CurrentSession.Get<T>(id);
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return Find<T>().SingleOrDefault(predicate);
        }

        public IQueryable<T> Find<T>() where T : IEntity
        {
            return _unitOfWork.CurrentSession.Query<T>();
        }

        public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return Find<T>().Where(predicate);
        }

        public T Add<T>(T entity) where T : IEntity
        {
            _unitOfWork.CurrentSession.Save(entity);
            return entity;
        }

        public T Remove<T>(T entity) where T : IEntity
        {
            _unitOfWork.CurrentSession.Delete(entity);
            return entity;
        }

        public void Remove<T>(object id) where T : IEntity
        {
            Remove(Load<T>(id));
        }
    }
}
