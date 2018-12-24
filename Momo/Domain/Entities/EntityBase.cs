using System;

namespace Momo.Domain.Entities
{
    public abstract class EntityBase : Entity
    {
        public virtual int Version { get; protected internal set; }
    }
}
