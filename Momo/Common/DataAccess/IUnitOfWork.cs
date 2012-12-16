using System;

namespace Momo.Common.DataAccess
{
    /// <summary>
    /// Wraps all data access in a transaction. The implementing unit of work class will define
    /// and expose its context (ISession for nhibernate for example). The unit of work should
    /// start automatically the first time the context is accessed. Thus, calling Commit, Rollback,
    /// or Dispose on a non-started unit of work should have no effect.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>Flush all pending modifications in the current context to the database and immediately begin a new unit of work</summary>
        void Commit();

        /// <summary>Discard all pending modifications in the current context and immediately begin a new unit of work</summary>
        void Rollback();
    }
}
