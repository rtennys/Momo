using System;
using System.Linq;
using System.Linq.Expressions;
using Momo.Domain;

namespace Momo.UI.Infrastructure
{
    public sealed class NHibernateRepository : IRepository
    {
        public NHibernateRepository(INHibernateUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly INHibernateUnitOfWork _unitOfWork;

        public T Load<T>(int id) where T : Entity
        {
            return _unitOfWork.CurrentSession.Load<T>(id);
        }

        public T Get<T>(int id) where T : Entity
        {
            return _unitOfWork.CurrentSession.Get<T>(id);
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : Entity
        {
            return Find<T>().SingleOrDefault(predicate);
        }

        public IQueryable<T> Find<T>() where T : Entity
        {
            return _unitOfWork.CurrentSession.Query<T>();
        }

        public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : Entity
        {
            return Find<T>().Where(predicate);
        }

        public T Add<T>(T entity) where T : Entity
        {
            _unitOfWork.CurrentSession.Save(entity);
            return entity;
        }

        public T Remove<T>(T entity) where T : Entity
        {
            _unitOfWork.CurrentSession.Delete(entity);
            return entity;
        }

        public void Remove<T>(int id) where T : Entity
        {
            Remove(Load<T>(id));
        }
    }
}
