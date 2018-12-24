using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Momo.Domain
{
    public abstract class Entity : IEquatable<Entity>
    {
        private int? _cachedHashCode;

        public virtual int Id { get; protected set; }

        public virtual bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var bothAreNew = IsTransient() && other.IsTransient();
            return !bothAreNew && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            if (_cachedHashCode == null)
                _cachedHashCode = IsTransient() ? RuntimeHelpers.GetHashCode(this) : Id.GetHashCode();

            return _cachedHashCode.Value;
        }

        private bool IsTransient()
        {
            return Id == 0;
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }
    }
}
