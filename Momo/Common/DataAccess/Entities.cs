using System;

namespace Momo.Common.DataAccess
{
    public interface IEntity
    {
        object Id { get; }
    }

    public class Entity : IEntity, IEquatable<Entity>
    {
        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }

        private int? _hashCode;

        public virtual object Id { get; protected set; }

        public virtual bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var bothAreNew = Equals(Id, null) && Equals(other.Id, null);
            return !bothAreNew && Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        public override int GetHashCode()
        {
            if (_hashCode.HasValue) return _hashCode.Value;
            return !Equals(Id, null) ? Id.GetHashCode() : (_hashCode = base.GetHashCode()).Value;
        }
    }

    public abstract class Entity<PK> : Entity
    {
        public new virtual PK Id
        {
            get { return base.Id == null ? default(PK) : (PK)base.Id; }
            protected set { base.Id = value; }
        }
    }
}