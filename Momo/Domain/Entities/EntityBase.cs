using System;
using Momo.Common.DataAccess;

namespace Momo.Domain.Entities
{
    public abstract class EntityBase : Entity<int>
    {
        public virtual int Version { get; protected internal set; }
    }
}
