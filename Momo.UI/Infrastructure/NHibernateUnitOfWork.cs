using System;
using Momo.Common.DataAccess;
using NHibernate;

namespace Momo.UI.Infrastructure
{
    public interface INHibernateUnitOfWork : IUnitOfWork
    {
        ISession CurrentSession { get; }
    }

    public class NHibernateUnitOfWork : INHibernateUnitOfWork
    {
        public NHibernateUnitOfWork(INHibernateSessionFactoryHelper sessionFactoryHelper)
        {
            _sessionFactoryHelper = sessionFactoryHelper;
        }

        private readonly INHibernateSessionFactoryHelper _sessionFactoryHelper;
        private ISession _session;

        public ISession CurrentSession
        {
            get { return _session ?? (_session = OpenSession()); }
        }

        public void Commit()
        {
            if (_session == null) return;

            _session.Transaction.Commit();
            _session.BeginTransaction();
        }

        public void Rollback()
        {
            if (_session == null) return;

            _session.Transaction.Rollback();
            _session.BeginTransaction();
        }

        public void Dispose()
        {
            if (_session == null) return;

            _session.Dispose();
            _session = null;
        }

        private ISession OpenSession()
        {
            var session = _sessionFactoryHelper.CurrentSessionFactory.OpenSession();

            session.FlushMode = FlushMode.Commit;
            session.BeginTransaction();

            return session;
        }
    }
}
