using System;

namespace Momo.Domain.Entities
{
    public class ShoppingList : EntityBase
    {
        protected ShoppingList()
        {
        }

        internal ShoppingList(UserProfile userProfile, string name)
        {
            UserProfile = userProfile;
            Name = name;
        }

        public virtual UserProfile UserProfile { get; protected set; }
        public virtual string Name { get; protected internal set; }
    }
}
